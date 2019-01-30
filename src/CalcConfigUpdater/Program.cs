using LiteDB;
using NTMiner.ServiceContracts.ControlCenter.DataObjects;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace NTMiner {
    class Program {
        static void Main(string[] args) {
            try {
                NTMinerRegistry.SetAutoBoot("NTMiner.CalcConfigUpdater", true);
                const int minutes = 60 * 1000;
                Timer t = new Timer(10 * minutes) {
                    Enabled = true
                };
                t.Elapsed += (object sender, ElapsedEventArgs e) => {
                    UpdateAsync();
                };
                t.Start();
                UpdateAsync();
                Console.WriteLine("输入exit并回车可以停止服务！");

                while (Console.ReadLine() != "exit") {
                }

                Console.WriteLine("服务停止成功: {0}.", DateTime.Now);
            }
            catch (Exception e) {
                PrintError(e);
            }

            System.Threading.Thread.Sleep(1000);
        }

        private static void UpdateAsync() {
            Task.Factory.StartNew(() => {
                try {
                    byte[] htmlData = GetHtmlAsync("https://www.f2pool.com/").Result;
                    if (htmlData != null && htmlData.Length != 0) {
                        Console.WriteLine($"{DateTime.Now} - 鱼池首页html获取成功");
                        string html = Encoding.UTF8.GetString(htmlData);
                        double usdCny = PickUsdCny(html);
                        Console.WriteLine($"usdCny={usdCny}");
                        List<IncomeItem> incomeItems = PickIncomeItems(html);
                        FillCny(incomeItems, usdCny);
                        NeatenSpeedUnit(incomeItems);
                        foreach (IncomeItem incomeItem in incomeItems) {
                            Console.WriteLine(incomeItem.ToString());
                        }
                        Console.WriteLine(incomeItems.Count + "条");
                        if (incomeItems != null && incomeItems.Count != 0) {
                            Login();
                            GetCalcConfigsResponse response = Server.ControlCenterService.GetCalcConfigs();
                            HashSet<string> coinCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            foreach (CalcConfigData calcConfigData in response.Data) {
                                IncomeItem incomeItem = incomeItems.FirstOrDefault(a => string.Equals(a.CoinCode, calcConfigData.CoinCode, StringComparison.OrdinalIgnoreCase));
                                if (incomeItem != null) {
                                    coinCodes.Add(calcConfigData.CoinCode);
                                    calcConfigData.Speed = incomeItem.Speed;
                                    calcConfigData.SpeedUnit = incomeItem.SpeedUnit;
                                    calcConfigData.IncomePerDay = incomeItem.IncomeCoin;
                                    calcConfigData.IncomeUsdPerDay = incomeItem.IncomeUsd;
                                    calcConfigData.IncomeCnyPerDay = incomeItem.IncomeCny;
                                    calcConfigData.ModifiedOn = DateTime.Now;
                                }
                            }
                            Server.ControlCenterService.SaveCalcConfigsAsync(response.Data);
                            Console.WriteLine($"更新了{string.Join(",", coinCodes)}");
                        }
                    }
                }
                catch (Exception e) {
                    PrintError(e);
                }
            });
        }

        private static void Login() {
            // 约定收益计算器运行在NTMinerServer程序的子目录
            string ntMinerServerLocalDbFileFullName = Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "local.litedb");
            using (var db = new LiteDatabase($"filename={ntMinerServerLocalDbFileFullName};journal=false")) {
                var col = db.GetCollection<UserData>();
                UserData user = col.FindOne(Query.All());
                if (user != null) {
                    Server.LoginName = user.LoginName;
                    Server.Password = user.Password;
                    Console.WriteLine($"LoginName:{user.LoginName}");
                    Console.Write($"Password:");
                    Console.ForegroundColor = Console.BackgroundColor;
                    Console.WriteLine(user.Password);
                    Console.ResetColor();
                }
            }
        }

        private static void FillCny(List<IncomeItem> incomeItems, double usdCny) {
            foreach (var incomeItem in incomeItems) {
                incomeItem.IncomeCny = usdCny * incomeItem.IncomeUsd;
            }
        }

        private static void NeatenSpeedUnit(List<IncomeItem> incomeItems) {
            foreach (var incomeItem in incomeItems) {
                if (!string.IsNullOrEmpty(incomeItem.SpeedUnit)) {
                    incomeItem.SpeedUnit = incomeItem.SpeedUnit.ToLower();
                    incomeItem.SpeedUnit = incomeItem.SpeedUnit.Replace("sol/s", "h/s");
                    incomeItem.SpeedUnit = incomeItem.SpeedUnit.Replace("graph/s", "h/s");
                }
            }
        }

        private static List<IncomeItem> PickIncomeItems(string html) {
            try {
                List<IncomeItem> results = new List<IncomeItem>();
                if (string.IsNullOrEmpty(html)) {
                    return results;
                }
                string patternFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pattern.txt");
                if (!File.Exists(patternFileFullName)) {
                    return results;
                }
                string pattern = File.ReadAllText(patternFileFullName);
                if (string.IsNullOrEmpty(pattern)) {
                    return results;
                }
                List<int> indexList = new List<int>();
                const string splitText = "<div class=\"row-collapse-container\" data-code=";
                int index = html.IndexOf(splitText);
                while (index != -1) {
                    indexList.Add(index);
                    index = html.IndexOf(splitText, index + splitText.Length);
                }
                Regex regex = new Regex(pattern, RegexOptions.Compiled);
                for (int i = 0; i < indexList.Count; i++) {
                    IncomeItem incomeItem;
                    if (i + 1 < indexList.Count) {
                        incomeItem = PickIncomeItem(regex, html.Substring(indexList[i], indexList[i + 1] - indexList[i]));
                    }
                    else {
                        incomeItem = PickIncomeItem(regex, html.Substring(indexList[i], 2000));
                    }
                    if (incomeItem != null) {
                        results.Add(incomeItem);
                    }
                }
                return results;
            }
            catch (Exception e) {
                PrintError(e);
                return new List<IncomeItem>();
            }
        }

        private static IncomeItem PickIncomeItem(Regex regex, string html) {
            Match match = regex.Match(html);
            if (match.Success) {
                IncomeItem incomeItem = new IncomeItem() {
                    DataCode = match.Groups["dataCode"].Value,
                    CoinCode = match.Groups["coinCode"].Value,
                    SpeedUnit = match.Groups["speedUnit"].Value
                };
                double speed = 0;
                double.TryParse(match.Groups["speed"].Value, out speed);
                incomeItem.Speed = speed;
                double incomeCoin = 0;
                double.TryParse(match.Groups["incomeCoin"].Value, out incomeCoin);
                incomeItem.IncomeCoin = incomeCoin;
                double incomeUsd = 0;
                double.TryParse(match.Groups["incomeUsd"].Value, out incomeUsd);
                incomeItem.IncomeUsd = incomeUsd;
                return incomeItem;
            }
            return null;
        }

        private static double PickUsdCny(string html) {
            try {
                double result = 0;
                Regex regex = new Regex(@"CURRENCY_CONF\.usd_cny = Number\('(\d+\.?\d*)' \|\| \d+\.?\d*\);");
                var matchs = regex.Match(html);
                if (matchs.Success) {
                    double.TryParse(matchs.Groups[1].Value, out result);
                }
                return result;
            }
            catch (Exception e) {
                PrintError(e);
                return 0;
            }
        }

        private static async Task<byte[]> GetHtmlAsync(string url) {
            try {
                using (WebClient client = new WebClient()) {
                    return await client.DownloadDataTaskAsync(new Uri(url));
                }
            }
            catch (Exception e) {
                PrintError(e);
                return new byte[0];
            }
        }

        private static void PrintError(Exception e) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message, e.StackTrace);
            Console.ResetColor();
        }
    }
}

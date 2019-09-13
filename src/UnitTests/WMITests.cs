using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Management;
using System.Text;

namespace UnitTestProject1 {
    [TestClass]
    public class WMITests {
        [TestMethod]
        public void TestMethod1() {
            ManagementObjectSearcher managementObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            var str = new StringBuilder();
            var keys = new[]
                {
                    "PNPDeviceID",
                    "AcceleratorCapabilities", //AcceleratorCapabilities  --图形和视频控制器的三维阵列的能力
                    "AdapterCompatibility", //AdapterCompatibility  --用于此控制器与系统比较兼容性一般芯片组
                    "AdapterDACType", //AdapterDACType  --姓名或数字 - 模拟转换器（DAC）芯片的标识符
                    "AdapterRAM", //AdapterRAM  --视频适配器的内存大小
                    "Availability", //Availability  --可用性和设备的状态
                    "CapabilityDescriptions", //CapabilityDescriptions  --自由形式字符串提供更详细的解释中的任何加速器能力阵列所指示的视频加速器功能
                    "Caption", //Caption  --对象的简短描述
                    "ColorTableEntries",  //尺寸系统的色表
                    "ConfigManagerErrorCode",  //Win32的配置管理器错误代码
                    "ConfigManagerUserConfig",  //如果为TRUE，该装置是使用用户定义的配置
                    "CreationClassName",  //第一个具体类的名称出现在创建实例所使用的继承链
                    "CurrentBitsPerPixel",  //使用的比特数以显示每个像素
                    "CurrentHorizontalResolution",  //水平像素的当前数量
                    "CurrentNumberOfColors",  //在当前的分辨率支持的色彩数目
                    "CurrentNumberOfColumns",  //此视频控制器列（如果在字符模式下）编号
                    "CurrentNumberOfRows",  //此视频控制器行（如果在字符模式下）编号
                    "CurrentRefreshRate",  //频率在该视频控制器刷新监视器的图像
                    "CurrentScanMode",  //当前扫描模式
                    "CurrentVerticalResolution",  //当前垂直像素数量
                    "Description",  //描述
                    "DeviceID", //DeviceID  --该视频控制器标识符（唯一的计算机系统）
                    "DeviceSpecificPens",  //目前许多设备专用笔。值0xFFFF表示设备不支持笔。
                    "DitherType",  //抖动型视频控制器
                    "DriverDate", //DriverDate  --当前已安装的视频驱动程序的最后修改日期和时间
                    "DriverVersion", //DriverVersion  --视频驱动程序的版本号
                    "ErrorCleared",  //如果为真，报上一个错误代码属性中的错误现已清除
                    "ErrorDescription",  //可能采取的纠正措施字符串提供有关记录在一个错误代码属性错误的详细信息
                    "ICMIntent",  //应使用默认三种可能的配色方法或意图中的一个特定值
                    "ICMMethod",  //处理ICM方法。对于非ICM的应用程序，这个属性决定了ICM是否已启用对于ICM的应用程序，系统将检查此属性来确定如何处理ICM支持
                    "InfFilename",  //视频适配器的路径.inf文件
                    "InfSection",  //Windows的视频信息所在的.inf文件
                    "InstallDate", //InstallDate  --安装的日期
                    "InstalledDisplayDrivers", //InstalledDisplayDrivers  --已安装的显示设备驱动程序的名称
                    "LastErrorCode",  //报告的逻辑设备上一个错误代码 
                    "MaxMemorySupported",  //以字节为单位支持的内存最高限额
                    "MaxNumberControlled",  //可支持通过该控制器可直接寻址的实体的最大数量
                    "MaxRefreshRate",  //在赫兹视频控制器的最大刷新率
                    "MinRefreshRate",   //在赫兹视频控制器的最小刷新率
                    "Monochrome",  //如果是TRUE，灰阶用于显示图像。
                    "Name",  //标签由该对象是已知的。当子类，该属性可以被覆盖是一个关键属性。
                    "NumberOfColorPlanes",  //当前一些颜色平面。如果该值不适用于当前视频的配置，输入0（零）
                    "NumberOfVideoPages",  //当前的分辨率和可用内存支持视频页数
                    "PNPDeviceID",  //即插即用逻辑设备的播放装置识别符
                    "PowerManagementCapabilities",  //逻辑设备的特定功率相关的能力阵列
                    "PowerManagementSupported",  //如果为TRUE，该装置可以是电源管理（可以投入挂起模式，等等）
                    "ProtocolSupported",  //由控制器使用协议访问“控制”的设备
                    "ReservedSystemPaletteEntries",  //系统调色板保留的条目数
                    "SpecificationVersion",  //初始化数据规范的版本号（在其上的结构的基础）
                    "Status",  //对象的当前状态
                    "StatusInfo",  //对象的当前状态详细信息
                    "SystemCreationClassName",  //该作用域计算机的创建类别名称属性的值
                    "SystemName",  //系统的名称
                    "SystemPaletteEntries",  //当前一些系统调色板颜色索引条目
                    "TimeOfLastReset",  //该控制器是最后一次复位日期和时间，这可能意味着该控制器被断电或重新初始化
                    "VideoArchitecture",  //视频体系结构的类型
                    "VideoMemoryType", //VideoMemoryType  --显存类型
                    "VideoMode",  //当前视频模式
                    "VideoModeDescription",  //当前的分辨率，颜色和视频控制器的扫描模式设置
                    "VideoProcessor"  //无格式的字符串描述视频处理器
                };
            foreach (ManagementObject m in managementObject.Get()) {
                foreach (var temp in keys) {
                    str.Append(temp);
                    str.Append(": ");
                    str.Append(m[temp]?.ToString() ?? "");
                    str.Append("\n");
                }
            }
            Console.WriteLine(str);
        }
    }
}

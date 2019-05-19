using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NTMiner.Wpf {
    public static class DataGridExtesnsion {
        /// <summary>
        /// Method checks whether the mouse is on the required Target
        /// Input Parameter (1) "Visual" -> Used to provide Rendering support to WPF
        /// Input Paraneter (2) "User Defined Delegate" positioning for Operation
        /// </summary>
        /// <param name="target"></param>
        /// <param name="getPos"></param>
        /// <returns>The "Rect" Information for specific Position</returns>
        private static bool IsTheMouseOnTargetRow(Visual target, Func<IInputElement, Point> getPos) {
            Rect posBounds = VisualTreeHelper.GetDescendantBounds(target);
            Point theMousePos = getPos((IInputElement)target);
            return posBounds.Contains(theMousePos);
        }

        /// <summary>
        /// Returns the selected DataGridRow
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static DataGridRow GetDataGridRowItem(int index, DataGrid dg) {
            if (dg.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                return null;
            }

            return dg.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        /// <summary>
        /// Returns the Index of the Current Row.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int GetDataGridRowIndex(this DataGrid dg, Func<IInputElement, Point> pos) {
            int curIndex = -1;
            for (int i = 0; i < dg.Items.Count; i++) {
                DataGridRow item = GetDataGridRowItem(i, dg);
                if (IsTheMouseOnTargetRow(item, pos)) {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
    }
}

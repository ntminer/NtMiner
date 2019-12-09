namespace NTMiner {
    /// <summary>
    /// 挖矿端主界面是透明且AllowsTransparency的，导致通过调整Opacity
    /// 模拟遮罩的时候因为透明能看到下面的桌面，所以有了这个接口。
    /// </summary>
    public interface IMaskWindow {
        void ShowMask();
        void HideMask();
    }
}

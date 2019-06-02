# 关于A卡超频

开源矿工很早就支持了N卡超频，不依赖小飞机且支持阉割版的矿卡系列，因为使用的是流出自NVIDA内部的超频工具，所以N卡超频是可信可靠的。但一直没有支持A卡超频，主要是因为网上找不到满意的适合嵌入进开源矿工的A卡超频工具，伟人说：自己动手丰衣足食，中国人从来都不比老外弱，所以还是自己动手吧。

在实现过程中发现，超频一直是AMD的强项，是AMD的一件历史悠久的事情，所以超频接口也是稳定可靠且版本兼容的，这一点应该给AMD个赞。
[这是AMD的文档](https://github.com/GPUOpen-LibrariesAndSDKs/display-library)
[用到的相关接口](https://github.com/ntminer-project/ntminer/blob/master/src/NTMiner.Core/Core/Gpus/Impl/Amd/AdlNativeMethods.cs)

已实现并上线，自版本号NTMiner 2.2.0.0往后都支持A卡超频。
# 需求

大部分程序会有一些需要周期执行的事情，也有一些需要在程序启动后执行一次的事情。很明显这类事情和时间有关，因为它们都需要在特定时间发生。这类需要执行的逻辑可以由时间事件发生器触发。

## 时间事件发生器
时间事件发生器是挂载在虚拟根[VirtualRoot](<https://github.com/ntminer-project/ntminer/blob/master/src/NTMinerlib/VirtualRoot.cs>)上的一个组件，它负责在特定的时间将特定类型的事件发布到系统总线上去。时间事件发生器会发布的事件有：

[代码位置](https://github.com/ntminer-project/ntminer/blob/master/src/NTMinerlib/Messages.cs)

```
HasBoot1SecondEvent
HasBoot2SecondEvent
HasBoot5SecondEvent
HasBoot10SecondEvent
HasBoot20SecondEvent
HasBoot1MinuteEvent
HasBoot2MinuteEvent
HasBoot5MinuteEvent
HasBoot10MinuteEvent
HasBoot20MinuteEvent
HasBoot50MinuteEvent
HasBoot100MinuteEvent
HasBoot24HourEvent

Per1SecondEvent
Per2SecondEvent
Per5SecondEvent
Per10SecondEvent
Per20SecondEvent
Per1MinuteEvent
Per2MinuteEvent
Per5MinuteEvent
Per10MinuteEvent
Per20MinuteEvent
Per50MinuteEvent
Per100MinuteEvent
Per24HourEvent
```

以上26种时间事件的设计灵感来源于人民币币值的设计，具体为什么是这些数值我们不做深究，实践证明这些数值正是开源矿工所需要的。

## 由谁发出这些时间事件?

可以看出，这类时间事件不属于任何模块，它们是由硬件系统的cpu发出的，所以对于我们的系统来说应该把时间事件发生器放在我们的系统的虚拟根[VirtualRoot](<https://github.com/ntminer-project/ntminer/blob/master/src/NTMinerlib/VirtualRoot.cs>)上。

## 这些消息发送到什么地方去?
发布到系统总线上去。每一个系统都应该有自己内部的系统总线，时间事件消息正是发布到系统自己内部的总线上去的。系统内部的各组件根据自己的需求从总线上订阅事件修建事件消息所行走的路径。

##力：走那条线程驱动这类事件?

如果你的系统是单线程的系统就不存在这个问题了，如果是多线程的系统尤其是带界面的系统的话这会是一个问题。根据自己的情况，这类事件可以由UI主线程驱动也可以由非UI线程驱动，值得注意的是如果这个事件发生器由非UI线程驱动的话订阅这些消息的程序需要处处小心自己的部分逻辑是否需要在UI上执行，如果这个发生器由UI线程驱动的话订阅这些事件的程序也要处处小心自己的逻辑是否会耗时超过50毫秒。
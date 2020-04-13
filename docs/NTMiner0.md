# 我们相信所有的系统都是从0开始的

## 我们相信，所有的系统都是从0开始的。
所谓从0开始就是从虚无开始，软件系统一定也是从0开始的。0可以称作Void、Virtual什么的，Void已经被编程语言占用了，那么我们用Virtual吧。
## 我们相信，所有的系统在空间结构上都是树形的，软件系统肯定不例外。
树是一个奇妙的结构，只要你愿意你所掌握的一切知识都是树形，你未掌握的知识也是树形。不做多说，[开源矿工](https://github.com/ntminer/NtMiner)系统在空间结构上和行为结构上都要有Root（根）概念。[开源矿工](https://github.com/ntminer/NtMiner)有根，构建[开源矿工](https://github.com/ntminer/NtMiner)的编程语言有根，[开源矿工](https://github.com/ntminer/NtMiner)所生存的运行时环境也有根，下层的事物我们不做追究，[开源矿工](https://github.com/ntminer/NtMiner)作为生存在操作系统时空中的一个小小的应用系统来说只感知自己所需要感知的环境即可。
## 我们相信，所有的词汇都应该按照望文生义理解
你望文生义出来的意思就是本真的意思，凡是不能望文生义的或者望文生义出的意思和联想不一致的语言文字载体都已经被前人丢弃或者迟早被后人丢弃。我们不使用不能望文生义的词汇。

从源代码的[VirtualRoot](https://github.com/ntminer/NtMiner/blob/master/src/NTMinerlib/VirtualRoot.cs)类型开始。望文生义，这是虚无，这是根。这是第一个出现的东西，它处在[开源矿工](https://github.com/ntminer/NtMiner)的最底层，所有上层建筑都建立在它之上。它下面肯定也有东西，但那是编程语言和运行环境的世界，我们的世界从VirtualRoot开始。
## VirtualRoot
VirtualRoot是个静态类型，它不是被我们构建而生的，有世界的时候它就已经在那里了，它是从0开始的0，它是整个世界的根。VirtualRoot根上挂载的事物也全都是静态的在世界开始的时候就已经在那里的事物，直接挂载在VirtualRoot上的事物有：
1. JsonSerializer
它是粉碎机，它是重建器，它安装在系统的出入口处，[开源矿工](https://github.com/ntminer/NtMiner)内部的物体流出系统前会被它打碎成下层系统的物体，下层原子世界不需要我们关注；外部系统的事物在进入我们的系统后首先会被它重建成我们的世界里事物的样子。我们的系统只有一个入口和一个出口，这唯一的出入口就是VirtualRoot，是根。
2. MessageDispatcher
推进器、动力源，或者其它什么名字，照此理解就可以了，[开源矿工](https://github.com/ntminer/NtMiner)系统内部运动的动力之源就是它，至于它的动力又是来自CPU这种下层世界的事物又超出我们的世界之外去了。
3. CommandBus
命令电车，它由MessageDispatcher驱动，MessageDispatcher不是装在它里面的引擎，MessageDispatcher是电。它运载命令，命令是一种消息，消息是一种空间结构体。
4. EventBus
事件电车，它由MessageDispatcher驱动，MessageDispatcher不是装在它里面的引擎，MessageDispatcher是电。它运载事件，事件是一种消息，消息是一种空间结构体。
## Message
上面知道VirtualRoot上挂在的CommandBus和EventBus分别是运载命令和事件的，而命令和事件是两种不同类型的消息。消息是什么？消息是消息的收发方所协定的承载信息的空间结构体，空间结构体是什么？就是一段树枝，树枝的枝杈相对位置和长短编码了能被收发方所理解的信息。
### Command
事情发生前的消息。
### Event
事情发生后的消息。
## Path
路径，消息所运动的路径。
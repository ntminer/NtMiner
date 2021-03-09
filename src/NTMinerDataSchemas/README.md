* 可以认为该程序集里的所有类型都是数据，都是可序列化的，极个别除外；
* 该程序集中除抽象类型、静态类型、Attribute类型和非NTMiner定义的类型外的类型的属性都是公共可读写的，且都有公共默认构造函数，因为它们都是可序列化的数据；
* UnitTests/NTMinerDataSchemasTests.cs中有个测试方法在保证以上要求；
* 凡是不在NTMinerDataSchemas中的类型都没有承诺过可序列化；
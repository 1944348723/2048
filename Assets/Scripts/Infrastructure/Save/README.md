# 简介
模仿`EasySave`接口和功能实现的存储系统

# 序列化
目前只实现了json序列化，且还是用的Unity自带的`JsonUtility`，由于`JsonUtility`不支持`int`、`float`这样的基础类型以及数组之类的，所以在序列化之前统一将数据装到`Wrapper<T>`中，这样基础数据类型就能存了，并让嵌套类型格式和基础类型一致

```C#
[Serializable]
class Wrapper<T> {
    public T data;
    public Wrapper<T>(T data) {
        this.data = value;
    }
}

[Serializable]
class ExampleData {
    public int data1 = 1;
    public float data2 = 1.1f;
    public bool data3 = true;
    public string data4 = "string";
}
serializer.Serialize(new ExampleData());
转出来的数据格式大概像下面这样
{"data":{"data1":1,"data2":1.1,"data3":true,"data4":"string"}}

int data = 1;
serializer.Serailize(data);
转出来的数据是这样
{"data":1}

也就是转出来的数据外面会套一层`{"data":};
```

# 存取
## PlayerPrefs
序列化后直接存入，一个`key`对应一项

取出时如果找不到`key`或者数据为空，则返回默认值

## JsonFile
JsonFile的存取稍微麻烦点，因为是在同一个文件中存多个`key`对应的数据，所以需要进行`key`的管理。定义了两个数据结构来完成Json结构管理，`SaveFileData`对应一整个文件的内容，`SaveEntry`对应一个`key-value`对，一个文件中有多个`key-value`对，所以`SaveFileData`中放的是`SaveEntry`列表。两个类都用了`[Serializable]`，所以实际存取时只要使用这两个数据结构来操作就行了，它们对应的序列化和反序列化操作会自动在`Serializer`中完成

```C#
[Serializable]
internal class SaveEntry
{
    public string Key;
    public string Value;
}

[Serializable]
internal class SaveFileData
{
    public List<SaveEntry> Entries = new();
}
```
实际一个文件中存储的数据会是这样的
```json
{"Entries":[{"Key":"HighScore","Value":"{\"data\":1476}"}]}
```
顶层是`Entries`，包含多个`Key-Value`对，`Value`中是数据序列化的内容，由于我们使用了`Wrapper<T>`包了一层，所以实际数据外面多了一层`data`

# 考虑的问题
## null的处理
对null的处理是在入口处直接抛异常拦截，我觉得传null在语义上就是不对的
```C#
Save(key, null);    // 这不就是不存数据吗，那其实根本就没必要调用
Save(null, data);   // null作为key我觉得不成立
Load(key);          // 不成立
```

`Load`时找不到的情况则返回默认值

## 写存档
如果直接在原存档文件上写，那么如果中途出现意外导致中断(如断电、游戏崩溃、磁盘异常)，那么原来的存档就可能损坏

常见的作法是先写到临时文件里，临时文件写入成功后删除原文件，然后重命名临时文件

不过`File`中提供了现成的`Replace(tmp, target, backup)`接口，已有存档时可以使用这个接口，没有时就用`Move()`重命名文件
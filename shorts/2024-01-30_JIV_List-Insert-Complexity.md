# List.Insert performance
Did you know that `List.Insert` method doesn't have same performance as `List.Add`? For example:

```csharp
void MoveItemUp<T>(List<T> list, int index)
{
    if (index > 0 && index < list.Count)
    {
        T item = list[index];
        list.RemoveAt(index);
        list.Insert(index - 1, item);
    }
}
```

Under the hood, `List` is implemented as [growable array](https://en.wikipedia.org/wiki/Dynamic_array). `Insert` method is [implemented](https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs,770) by copying all items after target index to make space for inserted item. Similarly, `RemoveAt` copy all items after index. So, `MoveItemUp` have actually linear *O(n)* complexity.

Constant time *O(1)* algorithm:

```csharp
void MoveItemUp<T>(List<T> list, int index)
{
    if (index > 0 && index < list.Count)
    {
        T item = list[index];
        list[index] = list[index - 1];
        list[index - 1] = item;
    }
}
```

---

> Found a bug or have additional questions? Let me know in the comments! I created this post on behalf of the CWE **SWAT Workgroup**. You can reach me and other group members at swat@ciklum.com.
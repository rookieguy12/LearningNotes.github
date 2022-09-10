## Meta Programming notes

```c++
```

### SFINAE

```c++
// My examples：要求禁止实参推导，以阻止指定仅integral的参数可以进入函数test
// 写法1
template<typename T>
typename enable_if<is_integral<T>::value, T>::type
test(typename std::remove_reference<T&>::type x) {
    cout << "yes";
    return 1;
}

// 写法2
template<typename T, typename enable_if<is_integral<T>::value, T>::type* = nullptr>
test(typename std::remove_reference<T&>::type x) {
    cout << "yes";
    return 1;
}

// 写法3 —— 用现成的enable_if_t代替写法1
template<typename T>
enable_if_t<is_integral<T>::value, T>
test(typename std::remove_reference<T&>::type x) {
    cout << "yes";
    return 1;
}

// 不建议的写法
template<typename T, typename = is_integral<T>>>
test(typename std::remove_reference<T&>::type x) {
    cout << "yes";
    return 1;
}
```


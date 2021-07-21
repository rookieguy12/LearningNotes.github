public delegate void CallBack(); //無參委托
public delegate void CallBack<T1>(T1 arg); //單參委托
public delegate void CallBack<T1, T2>(T1 arg1, T2 arg2); //雙參委托
public delegate void CallBack<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3); //三參委托
public delegate void CallBack<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4); //四參委托
public delegate void CallBack<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5); //五參委托
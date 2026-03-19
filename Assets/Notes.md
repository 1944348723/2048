[TOC]

# 架构
## MVC (Model-View-Controller)
``` mermaid
graph TD
    View --获取数据更新显示--> Model
    Controller --根据输入操作模型--> Model
    Controller --通知更新视图--> View
```
* **Model**: 业务数据和规则
* **View**: 显示
* **Controller**: 根据输入操作模型，然后通知View层更新，View层从Model中取数据进行更新

## MVP (Model-View-Presenter)
### Passive View
``` mermaid
graph
    Presenter --根据输入操作模型--> Model
    Model --提供数据--> Presenter
    Presenter --通知更新视图--> View
    View --通知用户输入--> Presenter
```
* **Model**: 业务数据和规则
* **View**: 显示
* **Presenter**: 根据输入操作模型，然后根据Model层返回的结果控制View层更新

### Supervising Controller
```mermaid
graph
    SupervisingController --根据输入操作模型--> Model
    Model --提供数据--> SupervisingController
    SupervisingController--通知更新视图--> View
    View --通知用户输入--> SupervisingController
    View --获取数据更新视图--> Model
```
* **Model**: 业务数据和规则
* **View**: 显示
* **SupervisingController**: 根据输入操作模型，然后根据Model层返回的结果控制View层更新

将一些简单的同步直接放在View中，避免Presenter过大
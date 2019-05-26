# Monree Date  
# 本地部署  
因本UWP应用的目标版本和最低版本都是Windows10 1809版本（Build 17763）,所以部署前请确保Windows10版本已升级到1809或以上的版本，1803（Build 17134）及之前的版本将不受支持！ 
P.S. 本应用尚未提供应用数据本地存储，每次启动应用将被初始化。  

## 应用本地部署  
[安装包传送门](https://pan.baidu.com/s/1lLpfJMCBcnsaoauJ1kIGMQ) 提取码：  0i3v  
下载到本地后，先安装证书（文件后缀为.cer），将证书安装在“受信任的根证书安装机构”，之后打开安装包（文件后缀.msix）即可安装。  
## 解决方案本地部署
请下载完整的解决方案到本地，解压后打开使用Visual Studio打开“倒计时.sln”，点击Debug即可完成部署。  

### 可能出现的问题：  
#### 1.缺少VS工作负载，无法打开解决方案。  
（解决方法：打开Visual Studio Installer，在修改选项里的工作负载里选择“通用Windows平台开发”,安装。）  

#### 2.缺少相关的NuGet包。  
（解决方法：在VS中的“工具”-“Nuget包管理器”-“管理解决方案的Nuget程序包”-搜索并安装“Microsoft.UI.XAML”，安装完毕后重新生成解决方案。）  
  
    
    
## 以下为简单介绍：  

代号：夏日（英文代号Monree Date）  

平台: [UWP](https://docs.microsoft.com/zh-cn/windows/uwp/get-started/universal-application-platform-guide)

语言：前台XAML + 后台C#  

设计：采用UWP平台最新的Fluent Design System（流畅设计体系），带来现代、美观、简洁的视觉效果。包括亚克力材质元素和背景、光标的光影效果、元素的深度效果，缩放以及动态效果还待加入。  
[流畅设计体系Fluent Design System](https://developer.microsoft.com/zh-cn/windows/apps/design?ocid=cxfluent-getstartedheader-devcenterappsdesign)


### 预计实现功能：  

1. 首页：今年已过天数以及进度条；已创建的日程信息（显示已过/还有XX天）；点击相关项显示具体内容；

2. 新建：在此页面输入日程名、选取时间，点击“新建”按钮，在首页完成日程的新建；

2. 计算：选取一个开始日期和一个结束日期，计算两者相差的天数等；

3. 节日：选取一年内的一些重要节日，显示当天距离所显示节日的日数差；

4. 设置：个人主页以及相关功能的设置；  

5. 实现登录;  

6. 首页：日程置顶功能；  

7. 时间线功能；  

8. 计算：排版优化；  


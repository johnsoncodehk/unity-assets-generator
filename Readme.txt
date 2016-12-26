1. 從Assets/Create/Assets Generator建立一個Generator, path 參數格式為"Assets/FolderA/FolderB"
2. 解決命名衝突: 將Plugins改名為Plugins~並刪除Plugins~.meta, 以~結尾的文件會被Unity忽略
3. 解決腳本丟失: 勾選copyMeta。如勾選，生成前必須先做第2步, 否則會出現meta重覆錯誤
4. 如有路徑衝突會彈窗提示選擇, Cancle:取消 Next:下一個 Select:選擇當前
5. 如更新了其中一個Plugins, 刪除之前生成的文件夾, 再重覆1-4重新生成(如有新檔案需要先把Plugins~改為Plugins讓Unity生成meta用作之後程序copy)

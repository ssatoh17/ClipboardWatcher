# ClipboardWatcher
クリップボードを監視して、SnagitのキャプチャURLの画像への直リンクを取得するツールです

## 本ツールを作ろうと思った背景

起動時画面
![起動時画面](https://content.screencast.com/users/ssatoh17/folders/Default/media/b73a589e-a213-488c-a7aa-c4047b7f3af4/08.03.2017-22.51.png "起動時画面")

画面左上には、直近のクリップボードにある画像が表示されていて、クリックすると画像としてクリップボードにコピーされます。
↓には文字列のクリップボード履歴が表示されます。左側に表示されているのは時刻です（連続でクリップボードイベントが発生してしまうことがあるため、そのデバッグのための意味が主です）。クリックした文字列がクリップボードにコピーされます。
![画面イメージ](https://content.screencast.com/users/ssatoh17/folders/Default/media/7eabcce3-234e-41c4-9734-b2659ef5c99b/08.03.2017-22.54.png "画面イメージ")


## 技術メモ

・クリップボードの監視は、Win32 API のラッパーを使用（ [C#|クリップボードの変更を監視する](http://anis774.net/codevault/clipboardwatcher.html) からほぼコピペ。‎@anis774さんありがとうございます）
・Snagitの画像への直リンクURLは、Webスクレイピングで取得している
・スクレイピングに使っているのは、WebClientクラスと、HTML Agility Pack

## 今後
マークダウン用の
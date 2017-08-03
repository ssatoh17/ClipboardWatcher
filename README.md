# ClipboardWatcher
クリップボードを監視して、SnagitのキャプチャURLの画像への直リンクを取得するツールです

- [ClipboardWatcher](#clipboardwatcher)
    - [本ツールを作ろうと思った背景](#本ツールを作ろうと思った背景)
    - [ツール紹介](#ツール紹介)
    - [技術メモ](#技術メモ)
    - [今後](#今後)
    - [その他](#その他)
        - [自分用メモ](#自分用メモ)

## 本ツールを作ろうと思った背景
画面キャプチャ画像を保存した時に、例えばLightShotだと

## ツール紹介

起動時画面
<!-- ![起動時画面](https://content.screencast.com/users/ssatoh17/folders/Default/media/b73a589e-a213-488c-a7aa-c4047b7f3af4/08.03.2017-22.51.png "起動時画面") -->
<img src="https://content.screencast.com/users/ssatoh17/folders/Default/media/b73a589e-a213-488c-a7aa-c4047b7f3af4/08.03.2017-22.51.png" width="400px">

画面左上には、直近のクリップボードにある画像が表示されていて、クリックすると画像としてクリップボードにコピーされます。
↓には文字列のクリップボード履歴が表示されます。左側に表示されているのは時刻です（連続でクリップボードイベントが発生してしまうことがあるため、そのデバッグのための意味が主です）。クリックした文字列がクリップボードにコピーされます。
<!-- ![画面イメージ](https://content.screencast.com/users/ssatoh17/folders/Default/media/7eabcce3-234e-41c4-9734-b2659ef5c99b/08.03.2017-22.54.png "画面イメージ") -->
<img src="https://content.screencast.com/users/ssatoh17/folders/Default/media/7eabcce3-234e-41c4-9734-b2659ef5c99b/08.03.2017-22.54.png" width="400px">


## 技術メモ

・**クリップボードの監視**は、Win32 API のラッパーを使用（ [C#|クリップボードの変更を監視する](http://anis774.net/codevault/clipboardwatcher.html) からほぼコピペ。‎@anis774さんありがとうございます）<br>
・Snagitの画像への直リンクURLは、Webスクレイピングで取得している<br>
・スクレイピングに使っているのは、WebClientクラス(HttpClientクラスでも良かったが）と、`HTML Agility Pack（＋Linq）`（参考サイト：[C#でURLのリソースからテキスト取得](http://www.katch.ne.jp/~h-inoue/tips/cs/0001.html)←何故かLightShotから取得できない。OpenReadメソッド実行時に403エラーが発生）<br>
参考サイト：[Html Agility Packを使ってWebページをスクレイピングするには？［C#、VB］](http://www.atmarkit.co.jp/ait/articles/1501/27/news140.html)


## 今後
マークダウン用の

<details><summary>detailsタグは使えるか？？</summary>htmlタグは使えるが、サービス側で制限している可能性もあり。GitHubでは使えた！</details>

<!-- 流石にスクリプトは使えなかった <button onclick="alert('hello!');">ボタン</button> -->

## その他
### 自分用メモ
https://github.com/ssatoh17/ClipboardWatcher

GitHubにGoogleアナリティクスを使える？？

GitHubのトップページに表示できるのは、mdファイルのみ？？index.htmlは使えない？
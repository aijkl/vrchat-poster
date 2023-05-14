# Twitter Poster

GCPのサービスアカウントやCloudFlare APIの認証情報が必要です。  
Twitter APIでは2次利用を許可しているのですが、本システムが適応されるのが曖昧であったため運用から1年経過して時点で運用を停止しました。  
		
## ポスターとは
私は[VRChat](https://hello.vrchat.com/)というゲームが好きで遊んでいます。  
VRChatはUnity製のユーザーがコンテンツを作って遊ぶ,VR型SNSです。  
ユーザーコンテンツは「[ワールド](https://qiita.com/henjiganai/items/b199a26e833a35f4a042)」と「[アバター](https://qiita.com/segur/items/34d1b0f71cd3dbfe27b5)」に分かれます。  
ワールドはVRChat社が開発した[Udon](https://docs.vrchat.com/docs/getting-started-with-udon)というノード型のプログラミング言語を用いて作成します。  
本ツールは**VRChatのワールドからツイッターを見れるようにするサービスのプログラムです。**


## 動作の仕組み
ワールドからは**制限されていますがインターネット**アクセスできます。  
**Webから写真をダウンロードしてテクスチャ―として使う**という機能があります。  

Twitter **APIからツイートを取得して写真を生成して配信**します。  
写真を壁に貼って**VRChatからTwitterを見る**ことができるというシステムです　　

![ポスター紹介](https://user-images.githubusercontent.com/51302983/152746371-89470035-3247-4671-b38b-16664fd94af7.png)　　
![ポスター紹介2](https://user-images.githubusercontent.com/51302983/152746480-b1c8162a-ade3-4594-afcc-a0fb9bdc7aba.png)　　

#VRChat_world紹介のツイートを掲載したり、VRChatが含まれていて、いいね数が100件以上のツイートを表示したりしていました。
	
[Twitter](https://github.com/aijkl/vrchat-poster/tree/master/Twitter)  
↑エントリポイントがあるメインプロジェクトです  
定期的にポーリングを行い写真を生成する、コンソールアプリです。  
Linuxで動作させるのが基本なため、Systemdが出力を保存するためログ機能は実装しませんでした。  
例外が発生した場合はDiscord APIを利用して例外情報をスマホに通知しています。
			
[PosterController](https://github.com/aijkl/vrchat-poster/tree/master/PosterController)  
↑管理用ツールです
設定ファイルが外部サービス上に配置されおり、ポスターを作る前に外部サービスから設定ファイルをダウンロードして使うという設計にしています。  
**PosterControllerでは設定ファイルを編集して配置**することが出来ます。  
ブラックリストや任意のツイートを挿入するプロモーション機能などを操作します。
			
上記以外のプロジェクトは上記のプロジェクトが参照しているプロジェクトです。  
実行可能形式になるプロジェクトは上記の2個です。
			
	
**写真の配信数は一ヶ月で200万回**ありましたがCDNを導入する事により、RaspberryPiで配信することが出来ました。  

![アクセス数](https://user-images.githubusercontent.com/51302983/152747116-75c0dc20-7163-46be-89b1-5f80341da99c.png)

Unityで2Dターン制コマンドバトルを制作しました。　
お借りしたツールとしてNewTonSoft.jsonとUniTaskをお借りして、二つのツールを用いて制作しました。

【操作方法】

<img width="400" height="340" alt="image" src="https://github.com/user-attachments/assets/b9768532-9649-453a-b35d-266a3384dec8" />

キーボードのSとAとFキーを使用してゲームをプレイします。

Aキーでは通常攻撃が発動し、Sキーではキャラクター固有のスキルを発動します。　Fキーは一定ターンで使用することのできる必殺技が発動することができます。　

そのほかにポーズ画面を表示するのにESC（エスケープキー）を押して開くことができ、途中でゲームを終了するときに使います。



<img width="400" height="340" alt="image" src="https://github.com/user-attachments/assets/deae9bb9-4804-4a58-9dd7-e35deae71bb5" />

画面右下にある剣マークのUIはキャラクターの状態を見たいときに押すことでキャラクターの状態、例えば体力や攻撃力を見ることができます。

【ゲームについて】
左から順番に行動し、敵のターンに代わり、同じように順番に行動します。
プレイヤーはスキルや必殺技を駆使してゲームクリアを目指します。

【プレイヤーキャラクターの紹介】


<img width="400" height="340" alt="image" src="https://github.com/user-attachments/assets/dbd8ea3d-24ef-44d8-aaf0-afc3afac93e7" />

【敵の紹介】

<img width="400" height="340" alt="image" src="https://github.com/user-attachments/assets/c44828c3-ad34-4dc8-8fa5-5da4c27aa955" />

<img width="400" height="340" alt="image" src="https://github.com/user-attachments/assets/47e7d187-547d-4a50-b55b-519b1866d508" />

【プログラムについて】
挑戦（チャレンジ）

UniTask＋Coroutine で非同期制御を導入
└バトル中の処理を非同期化

継承＋ScriptableObject＋JSON
└ステータスの管理を汎用化

成果

バトルシーケンスをメインスレッドを止めずに進行できるよう改善。
└演出やUI更新を止めない快適なゲーム体験を実現


開発効率の向上！
└新キャラクターの追加が継承とSOだけで完結

【プログラムでのバトルシステムについて】
ターン性コマンドバトル：戦略性×没入感

└結果：UniTask＋ Coroutineの非同期処理で演出やUI更新を止めない快適なゲーム体験を実現
UI表示とエフェクトの表示時間をフレームや秒数で管理し、一時停止などを非同期で行っています。


<img width="1625" height="426" alt="image" src="https://github.com/user-attachments/assets/755f4d12-c7b2-4ba6-9b23-f98b636c465e" />

⇑プレイヤーターンUIの表示をUniTaskを用いて数秒後にUIが非表示になるよう実装しています

【キャラクターステータス：再利用×拡張性】

「継承」

再利用性向上
└ 共通ロジックを親クラスで一元化

拡張も簡単
└新たな役職（◯◯クラス）を子クラス追加するだけ

細かなキャラ差別化
	 └支援A／強化Bなど、特定役割向けクラスを複数定義可能

<img width="549" height="340" alt="image" src="https://github.com/user-attachments/assets/3784360f-027b-4053-b4a3-d12fc92c706a" />


【 Scriptable Objectによるパラメータ管理の汎用化】

～アセットの汎用化×外部連携でメンテナンス負荷80%削減を実現～

<img width="549" height="340" alt="image" src="https://github.com/user-attachments/assets/26ade286-88f6-43ac-b403-32188a8a047a" />


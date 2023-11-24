using ankoPlugin2;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;


// # りーどみー
// 
// VisualStudio 2019 を使う
// .net Framework は 2022 ではビルド出来ない
//
// 参考サイト: https://seesaawiki.jp/yari_an/d/%a4%cf%a4%b8%a4%e1%a4%c6%a4%ce%a5%d7%a5%e9%a5%b0%a5%a4%a5%f3
//
//
//
// # 下準備 1
// アンコちゃんのフォルダーから以下の DLL を依存関係に追加する (プラグイン作成に必要)
//  * ankoPlugin2.dll
//  * LibAnko.dll
// 依存関係の追加方法は以下
//  * ツールバーの プロジェクト > プロジェクト参照の追加 > 左メニューの「参照」
//  * 下のボタンの「参照」をクリックして追加する DLL を選択する (複数選択可能)
//    HintPath に該当のDLLの相対/絶対パスを記述する
//
//
// # 下準備 2
// 1. AnkoChanPluginSample.csproj を開く
//    * 「ソリューションエクスプローラー」の AnckoChanLib をダブルクリックする
//      (ソリューションエクスプローラー が見つからない場合はツールバーの虫眼鏡から検索)
// 2. PropertyGroup の以下の項目を正しく記述する
//    * <StartProgram>この文をアンコちゃんの実行ファイルのパスに変更する</StartProgram>
// 3. ツールバーのデバッグ > オプション を開き「マイコードのみ」にチェックをつける
//    (これをしないと関係のない例外が出て面倒)
// 4. ツールバーの「緑色の三角」をクリックして実行する
// 5. アンコちゃんの設定を開き 「設定レベル」にチェックを付けてOKを押して再度開く
// 6. 設定の「上級者設定１」のタブで「プラグインフォルダー」の項目に以下を設定する
//    * [このプロジェクトフォルダ]\obj\Debug\net4.8-windows
//
// 
// # メモ
// [ブレークポイント]
// エディターの列番号の左をクリックすると ブレークポイント を設定できる
// ブレークポイントまで実行された場合には該当行で停止する
//
// ブレークポイントで停止中は変数にカーソルを合わせると中身が見える
// また、下の「ローカル」タブで現在の変数の中身を確認出来る
//
// [デバッグ停止時に利用可能なツールバーのアイコン]
// ステップイン
// * 関数で停止している場合は、関数の中に移動する. それ以外の場合は次の行まで実行する
// ステップオーバー
// * 次の行まで実行する. 関数の場合でも中身に移動せずに次の行まで実行する
// ステップアウト
// * 現在の関数の呼出元に戻るまで実行する
//
// [ホットリロード]
// 実行中にプログラムを編集/保存してホットリロード (ツールバーの火のマーク) をすると、
// プログラムを再実行せずにプログラムの内容を反映できる
// (一部ホットリロードが不可能な場合もあるのでその場合は再実行する)
//
//
// # プラグインのビルド
// ツールバーのビルド > ソリューションのビルド を選択するとDLLがビルドされる
// ビルド先は下の「出力」タブ (無い場合はツールバーの虫眼鏡から「出力」を検索) に表示される
// 実用する場合は実行ボタンの左の「Debug」を「Release」にしてビルドした方が良い?
// * Release にしたまま忘れていると、ブレークポイントが反映されないと表示されるのでDebugに戻すのを忘れない
// * Debug と Release はそれぞれ出力フォルダが違うので注意！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
//
//
// # メモ
// ブレークポイントが反応しない場合は以下の設定を確認！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
// ブレークポイントが反応しない場合は以下の設定を確認！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
// ブレークポイントが反応しない場合は以下の設定を確認！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
// ブレークポイントが反応しない場合は以下の設定を確認！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
// * 「Debug」になっているか確認
// * アンコちゃんの「上級者設定1」の「プラグインフォルダー」の設定
// 
// コンソール出力は使えないので代わりに Debug.Print() を利用する
// 出力タブに出力される
//

namespace AnkoChanPluginSample
{
    // ankoPlugin2.IPlugin インターフェースを実装する (class XX: IPlugin)
    public class Program : IPlugin
    {
        #region プラグイン用のプロパティ
        public string Name => "サンプルプラグイン";

        public string Description => "サンプルのプラグインです";

        public bool IsAlive => true;


        private IPluginHost _host;
        public IPluginHost host {
            get => _host;
            set {
                _host = value;

                // アンコちゃんから連携するためのオブジェクトを受け取ったら初期化
                SetUp();
            }
        }
        #endregion プラグイン用のプロパティ


        private Random random = new Random();



        public void Run()
        {
            //MessageBox.Show("実行！ a");
            Debug.Print("A");
        }

        private void SetUp()
        {
            // EventHandler には += で関数の追加. -= で関数の削除をする
            // 該当のイベントが実行された時に呼び出される
            _host.ReceiveChat += ReceiveChat;
            _host.ReceiveContentStatus += ReceiveContentStatus;
        }


        private void ReceiveChat(object sender, ReceiveChatEventArgs e)
        {
            //おまじない _host.CurrentCastがnullだったら処理しない (参考サイトより引用)
            if (_host.CurrentCast == null)
                return;



            // C# のテストサイト https://dotnetfiddle.net
            //  * 左側の Compiler で Latest を最初に選ぶと良い
            // C# (dotnet) の正規表現テスターサイト http://regexstorm.net/tester
            //  * 使い方
            //    * Pattern      正規表現
            //    * Input        テストする文字列
            //    * Replacement  置換文字 (下の Context 欄に置換後の文字が出力される)
            //  * 例
            //    * Pattern      (.*)1d([1-9][0-9]*)(.*)
            //    * Input        これは1d10だ
            //    * Replacement  $1_RND_$3
            //    * Context      これは_RND_だ
            // 正規表現メモ
            //  * () の中はキャプチャされ $n で取り出す
            //  * $0 の場合は全文字列. $n なら n 番目の () の文字

            // var は型を自動判定してくれる
            // なんでも入る変数ではなく、自動で型を判別してくれるだけなので、string の変数に int を入れたりは出来ない
            var comment = e.Chat.Message;

            // 正規表現で文字列を解析する. 戻り値は解析結果
            // () でキャプチャされたものは Groups で取り出せる. $n と同じ
            var match = Regex.Match(comment, "(.*)1d([1-9][0-9]*)(.*)");

            // 正規表現に一致しなかった場合は return
            if (!match.Success) return;


            // // 正規表現の2つ目のキャプチャ (1dX の X部分) を数値に変換する
            // if (!int.TryParse(match.Groups[2].Value, out int cnt)) return;
            // // int.TryParse の説明
            // // * 戻り値は数値に変換出来たかどうか (ここでは false の場合に何もしない)
            // // * 数値に変換出来た場合は cnt に数値が入る (出来なかった場合は 0 が入る)

            // でも正規表現に一致してる時点で失敗するはずがないので int.Parse で良い事に気がついた (かしこい)
            // (int.Parse の場合は失敗すると例外になる)
            var cnt = int.Parse(match.Groups[2].Value);


            // "" はただの文字列 $"" にすると式を {} で埋め込める文字列になる (式は値が返る構文. 式=変数/関数/計算式)
            //                   @"" は \ をエスケープしなくなる. $@"" も可
            var msg = $"{match.Groups[1].Value}{random.Next(0, cnt)}{match.Groups[3].Value}";

            //_host.PostComment(msg, ""); // PostComment("コメント", "コマンド")
            _host.PostOwnerComment(msg, "", "ああ");
            // コマンドは文字位置や色など. 半角スペースで区切って複数指定可能. 匿名のコマンドは184

        }

        private void ReceiveContentStatus(object sender, ReceiveContentStatusEventArgs e)
        {
            // TODO: このメソッドの意味を調べる (接続先の放送と接続アカウント情報?)
        }
    }
}

using ankoPlugin2;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

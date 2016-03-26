﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExtTests
{
    public class ParsecTests
    {
        [Fact]
        public void ResultComb()
        {
            var p = result(1234);
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
        }

        [Fact]
        public void ZeroComb()
        {
            var p = zero<Unit>();
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void ItemComb()
        {
            var p = anyChar;
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void ItemFailComb()
        {
            var p = anyChar;
            var r = parse(p, "");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void Item2Comb()
        {
            var p = anyChar;
            var r1 = parse(p, "Hello");

            Assert.False(r1.IsFaulted);
            Assert.True(r1.Reply.Result == 'H');
            Assert.True(r1.Reply.State.ToString() == "ello");

            var r2 = parse(p, r1.Reply.State);

            Assert.False(r2.IsFaulted);
            Assert.True(r2.Reply.Result == 'e');
            Assert.True(r2.Reply.State.ToString() == "llo");

        }

        [Fact]
        public void Item1LinqComb()
        {
            var p = from x in anyChar
                    select x;

            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void Item2LinqComb()
        {
            var p = from x in anyChar
                    from y in anyChar
                    select Tuple(x, y);

            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result.Item1 == 'H');
            Assert.True(r.Reply.Result.Item2 == 'e');
            Assert.True(r.Reply.State.ToString() == "llo");
        }

        [Fact]
        public void EitherFirstComb()
        {
            var p = either(ch('a'), ch('1'));

            var r = parse(p, "a");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'a');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void EitherSecondComb()
        {
            var p = either(ch('a'), ch('1'));

            var r = parse(p, "1");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == '1');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void EitherLINQComb()
        {
            var p = from x in either(ch('a'), ch('1'))
                    from y in either(ch('a'), ch('1'))
                    select Tuple(x, y);

            var r = parse(p, "a1");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result.Item1 == 'a');
            Assert.True(r.Reply.Result.Item2 == '1');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void UpperComb()
        {
            var p = upper;
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void UpperFailComb()
        {
            var p = upper;
            var r = parse(p, "hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void LowerComb()
        {
            var p = lower;
            var r = parse(p, "hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'h');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void LowerFailComb()
        {
            var p = lower;
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void DigitComb()
        {
            var p = digit;
            var r = parse(p, "1234");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == '1');
            Assert.True(r.Reply.State.ToString() == "234");
        }

        [Fact]
        public void DigitFailComb()
        {
            var p = digit;
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void LetterComb()
        {
            var p = letter;
            var r = parse(p, "hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'h');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void LetterFailComb()
        {
            var p = letter;
            var r = parse(p, "1ello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void WordComb()
        {
            var p = asString(many1(letter));
            var r = parse(p, "hello   ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "hello");
            Assert.True(r.Reply.State.ToString() == "   ");
        }

        [Fact]
        public void WordFailComb()
        {
            var p = asString(many1(letter));
            var r = parse(p, "1ello  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void StringMatchComb()
        {
            var p = str("hello");
            var r = parse(p, "hello world");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "hello");
            Assert.True(r.Reply.State.ToString() == " world");
        }

        [Fact]
        public void StringMatchFailComb()
        {
            var p = str("hello");
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void NaturalNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Natural;
            var r = parse(p, "1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void NaturalNumberFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Natural;
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void IntegerNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void IntegerNegativeNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "-1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == -1234);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void IntegerNumberFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void BracketAndIntegerComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.Brackets(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;

            var r = parse(p, "[1]  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndIntegerFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Brackets(tok.Integer);
            var r = parse(p, "[x]  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void BracketAndIntegerListComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.BracketsCommaSep(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;
            var r = parse(p, "[1,2,3,4]  ");

            Assert.False(r.IsFaulted);

            var arr = r.Reply.Result.ToArray();
            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndSpacedIntegerListComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.BracketsCommaSep(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;

            var r = parse(p, "[ 1, 2 ,3,   4]  ");

            Assert.False(r.IsFaulted);

            var arr = r.Reply.Result.ToArray();
            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndIntegerListFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.BracketsCommaSep(tok.Integer);
            var r = parse(p, "[1,x,3,4]  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void JunkEmptyComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, "");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkNoMatchComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, ",");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkFourSpacesComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, "    ,");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkFourSpacesThenCommentComb()
        {
            var tok = makeTokenParser(Language.JavaStyle);
            var p = tok.WhiteSpace;
            var r = parse(p, "    // A comment\nabc");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
            Assert.True(r.Reply.State.ToString() == "abc");
        }

        [Fact]
        public void StringLiteralComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.StringLiteral;
            var r = parse(p, "\"/abc\"");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "/abc");
        }

        [Fact]
        public void ActorConfigParserTest1()
        {
            var conftext = @"
            
                timeout:           30 seconds
                session-timeout:   60 seconds
                mailbox-size:      10000

                processes:
                    process:
                        pid:          ""/root/test/123""
                        flags:        [persist-inbox, persist-state, remote-publish]
                        mailbox-size: 1000

                        strategy:
                            one-for-one:

                                retries: count = 5, duration=30 seconds
                                back-off: min = 2 seconds, max = 1 hour, step = 5 seconds
                        
                                match
                                | System.NotImplementedException -> stop
                                | System.ArgumentNullException   -> escalate
                                | _                              -> restart

                                redirect when
                                | restart  -> forward-to-parent
                                | escalate -> forward-to-self
                                | stop     -> forward-to-process ""/root/test/567""
                                | _        -> forward-to-dead-letters

                        settings:
                            blah:    ""Something for the process to use""
                            another: ""Another setting""
            ";

            var res = parse(ActorConfigParser.Parser, conftext);

            Assert.False(res.IsFaulted);
            var conf = res.Reply.Result;
            var remain = res.Reply.State;

            Assert.True(conf.Pid.Path == "/root/te st/123");
            Assert.True((conf.Flags & ProcessFlags.PersistInbox) != 0);
            Assert.True((conf.Flags & ProcessFlags.PersistState) != 0);
            Assert.True((conf.Flags & ProcessFlags.RemotePublish) != 0);
            Assert.True(conf.MailboxSize == 1000);
            Assert.True(conf.Settings["blah"] == "Something for the process to use");
            Assert.True(conf.Settings["another"] == "Another setting");
            Assert.True(res.Reply.State.ToString() == "");
        }

        [Fact]
        public void ActorConfigParserFailTest1()
        {
            var conftext = @"

                pidy:          ""/root/test/123""
                flagsy:        [persist-inbox, persist-state, remote-publish]
                mailbox-sizey: 1000
                settingsy:
                    blah:    ""Something for the process to use""
                    another: ""Another setting""
            ";

            var res = parse(ActorConfigParser.Parser, conftext);

            Assert.True(res.IsFaulted);

            Assert.True(res.Reply.Error.Msg == "y");
            Assert.True(res.Reply.Error.Pos.Column + 1 == 20);
            Assert.True(res.Reply.Error.Pos.Line + 1 == 3);
            Assert.True(res.Reply.Error.Expected.Count == 1);
            Assert.True(res.Reply.Error.Expected.Head() == "end of pid");
        }

        [Fact]
        public void ActorConfigParserFailTest2()
        {
            var conftext = @"

                pid:          ""/root/test/123""
                flags:        [persist-inbox, PersisT-State, remote-publish]
                mailbox-size: 1000
                settings:
                    blah:    ""Something for the process to use""
                    another: ""Another setting""
            ";

            var res = parse(ActorConfigParser.Parser, conftext);

            Assert.True(res.IsFaulted);
            Assert.True(res.Reply.Error.Tag == ParserErrorTag.Expect);
            Assert.True(res.Reply.Error.Msg == "\"P\"");
            Assert.True(res.Reply.Error.Pos.Column == 46);
            Assert.True(res.Reply.Error.Pos.Line == 3);
        }

        [Fact]
        public void SettingTokenIntTest()
        {
            var text = @"my-setting: 123";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.Int("value"))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Int);
            Assert.True((int)setting.Values["value"].Value == 123);
        }

        [Fact]
        public void SettingTokenDoubleTest()
        {
            var text = @"my-setting: 123.45";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.Double("value"))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Name == "value");
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Double);
            Assert.True((double)setting.Values["value"].Value == 123.45);
        }

        [Fact]
        public void SettingTokenStringTest()
        {
            var text = @"my-setting: ""abc"" ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.String("value"))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Name == "value");
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.String);
            Assert.True((string)setting.Values["value"].Value == "abc");
        }

        [Fact]
        public void SettingTokenTimeTest()
        {
            var text = @"my-setting: 4 hours ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.Time("value"))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Name == "value");
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Time);
            Assert.True((Time)setting.Values["value"].Value == 4 * hours);
        }

        [Fact]
        public void SettingTokenProcessIdTest()
        {
            var text = @"my-setting: ""/root/user/blah"" ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.ProcessId("value"))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Name == "value");
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.ProcessId);
            Assert.True((ProcessId)setting.Values["value"].Value == new ProcessId("/root/user/blah"));
        }

        [Fact]
        public void SettingTokenProcessNameTest()
        {
            var text = @"my-setting: ""root-proc-name"" ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.ProcessName("value"))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Name == "value");
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.ProcessName);
            Assert.True((ProcessName)setting.Values["value"].Value == new ProcessName("root-proc-name"));
        }

        [Fact]
        public void SettingTokenProcessFlagsTest()
        {
            var text = @"my-setting: [persist-inbox, persist-state, remote-publish] ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.ProcessFlags("value"))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Name == "value");
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.ProcessFlags);

            var flags = (ProcessFlags)setting.Values["value"].Value;
            Assert.True((flags & ProcessFlags.PersistInbox) != 0);
            Assert.True((flags & ProcessFlags.PersistState) != 0);
            Assert.True((flags & ProcessFlags.RemotePublish) != 0);
        }

        [Fact]
        public void SettingTokenArrayIntTest()
        {
            var text = @"my-setting: [1,2,3 , 4] ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.Array("value", ArgumentType.Int))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Name == "value");
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Array);
            Assert.True(setting.Values["value"].Type.GenericType.Tag == ArgumentTypeTag.Int);

            var array = (Lst<int>)setting.Values["value"].Value;

            Assert.True(array.Count == 4);
            Assert.True(array[0] == 1);
            Assert.True(array[1] == 2);
            Assert.True(array[2] == 3);
            Assert.True(array[3] == 4);
        }

        [Fact]
        public void SettingTokenArrayStringTest()
        {
            var text = @"my-setting: [""hello"",""world""] ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.Array("value", ArgumentType.String))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 1);
            Assert.True(setting.Values["value"].Name == "value");
            Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Array);
            Assert.True(setting.Values["value"].Type.GenericType.Tag == ArgumentTypeTag.String);

            var array = (Lst<string>)setting.Values["value"].Value;

            Assert.True(array.Count == 2);
            Assert.True(array[0] == "hello");
            Assert.True(array[1] == "world");
        }

        [Fact]
        public void SettingTokenNamedArgsIntStringTest()
        {
            var text = @"my-setting: max = 123, name = ""hello"" ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting", ArgumentSpec.Int("max"), ArgumentSpec.String("name"))
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 2);

            var arg0 = setting.Values["max"];
            Assert.True(arg0.Name == "max");
            Assert.True(arg0.Type.Tag == ArgumentTypeTag.Int);
            Assert.True((int)arg0.Value == 123);

            var arg1 = setting.Values["name"];
            Assert.True(arg1.Name == "name");
            Assert.True(arg1.Type.Tag == ArgumentTypeTag.String);
            Assert.True((string)arg1.Value == "hello");
        }

        [Fact]
        public void SettingTokenNamedArgsIntStringArrayTest()
        {
            var text = @"my-setting: max = 123, name = ""hello"", coef = [0.1,0.3,0.5] ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting",
                        ArgumentSpec.Int("max"),
                        ArgumentSpec.String("name"),
                        ArgumentSpec.Array("coef", ArgumentType.Double)
                        )
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 3);

            var arg0 = setting.Values["max"];
            Assert.True(arg0.Name == "max");
            Assert.True(arg0.Type.Tag == ArgumentTypeTag.Int);
            Assert.True((int)arg0.Value == 123);

            var arg1 = setting.Values["name"];
            Assert.True(arg1.Name == "name");
            Assert.True(arg1.Type.Tag == ArgumentTypeTag.String);
            Assert.True((string)arg1.Value == "hello");

            var arg2 = setting.Values["coef"];
            Assert.True(arg2.Name == "coef");
            Assert.True(arg2.Type.Tag == ArgumentTypeTag.Array);
            Assert.True(arg2.Type.GenericType.Tag == ArgumentTypeTag.Double);

            var array = (Lst<double>)arg2.Value;
            Assert.True(array.Count == 3);
            Assert.True(array[0] == 0.1);
            Assert.True(array[1] == 0.3);
            Assert.True(array[2] == 0.5);
        }

        [Fact]
        public void SettingTokenNamedArgsIntArrayFlagsTest()
        {
            var text = @"my-setting: max = 123, name = ""hello"", coef = [0.1,0.3,0.5], flags = [listen-remote-and-local] ";

            var sys = new SystemConfigParser(
                    SettingSpec.Attr("my-setting",
                        ArgumentSpec.Int("max"),
                        ArgumentSpec.String("name"),
                        ArgumentSpec.ProcessFlags("flags"),
                        ArgumentSpec.Array("coef", ArgumentType.Double)
                        )
                    );

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var setting = res.Reply.Result["my-setting"];

            Assert.True(setting.Name == "my-setting");
            Assert.True(setting.Values.Length == 4);

            var arg0 = setting.Values["max"];
            Assert.True(arg0.Name == "max");
            Assert.True(arg0.Type.Tag == ArgumentTypeTag.Int);
            Assert.True((int)arg0.Value == 123);

            var arg1 = setting.Values["name"];
            Assert.True(arg1.Name == "name");
            Assert.True(arg1.Type.Tag == ArgumentTypeTag.String);
            Assert.True((string)arg1.Value == "hello");

            var arg2 = setting.Values["coef"];
            Assert.True(arg2.Name == "coef");
            Assert.True(arg2.Type.Tag == ArgumentTypeTag.Array);
            Assert.True(arg2.Type.GenericType.Tag == ArgumentTypeTag.Double);

            var arg3 = setting.Values["flags"];
            Assert.True(arg3.Name == "flags");
            Assert.True(arg3.Type.Tag == ArgumentTypeTag.ProcessFlags);
            Assert.True((ProcessFlags)arg3.Value == ProcessFlags.ListenRemoteAndLocal);
        }

        [Fact]
        public void ProcessesSettingsParserTest()
        {
            var text = @"
            
                timeout:           30 seconds
                session-timeout:   60 seconds
                mailbox-size:      10000

                strategies: [
                    my-strategy:
                        one-for-one:
                            retries: count = 5, duration=30 seconds
                            back-off: min = 2 seconds, max = 1 hour, step = 5 seconds
                        
                            match
                            | System.NotImplementedException -> stop
                            | System.ArgumentNullException   -> escalate
                            | _                              -> restart

                            redirect when
                            | restart  -> forward-to-parent
                            | escalate -> forward-to-self
                            | stop     -> forward-to-process ""/root/test/567""
                            | _        -> forward-to-dead-letters,

                    my-other-strategy:
                        one-for-one:
                            pause: 1 second
                            always: resume
                            redirect: forward-to-process ""/root/test/567""
                ]

                processes: [
                    pid:          ""/root/test/123""
                    flags:        [persist-inbox, persist-state, remote-publish]
                    mailbox-size: 1000
                    strategy:     @my-strategy,

                    pid:          ""/root/test/567""
                    flags:        [persist-inbox, persist-state]
                    mailbox-size: 100
                    strategy:     @my-other-strategy
                ]
                ";

            var strategy = new[] {
                SettingSpec.Attr("always", settings => Strategy.Always((Directive)settings["value"].Value),  ArgumentSpec.Directive("value")), 

                SettingSpec.Attr("pause", settings => Strategy.Pause((Time)settings["duration"].Value), ArgumentSpec.Time("duration")),

                SettingSpec.Attr("retries",
                    new ArgumentsSpec(
                        settings => Strategy.Retries((int)settings["count"].Value,(Time)settings["duration"].Value),
                        ArgumentSpec.Int("count"), ArgumentSpec.Time("duration")),

                    new ArgumentsSpec(
                        settings => Strategy.Retries((int)settings["count"].Value),
                        ArgumentSpec.Int("count"))
                        ),

                SettingSpec.Attr("back-off",
                    new ArgumentsSpec(
                        settings => Strategy.Backoff((Time)settings["min"].Value,(Time)settings["max"].Value,(Time)settings["step"].Value),
                        ArgumentSpec.Time("min"), ArgumentSpec.Time("max"), ArgumentSpec.Time("step")
                        ),

                    new ArgumentsSpec(
                        settings => Strategy.Backoff((Time)settings["duration"].Value),
                        ArgumentSpec.Time("duration")
                        )),

                SettingSpec.AttrNoArgs("match"),

                SettingSpec.AttrNoArgs("redirect")
            };

            var process = ArgumentType.Process(
                SettingSpec.Attr("pid", ArgumentSpec.ProcessId("value")),
                SettingSpec.Attr("flags", ArgumentSpec.ProcessFlags("value")),
                SettingSpec.Attr("mailbox-size", ArgumentSpec.Int("value")),
                SettingSpec.Attr("strategy", ArgumentSpec.Strategy("value", strategy)));

            var sys = new SystemConfigParser(
                SettingSpec.Attr("timeout", ArgumentSpec.Time("value")),
                SettingSpec.Attr("session-timeout", ArgumentSpec.Time("value")),
                SettingSpec.Attr("mailbox-size", ArgumentSpec.Int("value")),
                SettingSpec.Attr("settings", ArgumentSpec.Int("value")),
                SettingSpec.Attr("strategies", ArgumentSpec.Map("value", ArgumentType.Strategy(strategy))),
                SettingSpec.Attr("processes", ArgumentSpec.Array("value", process)));

            var res = parse(sys.Settings, text);

            Assert.False(res.IsFaulted);

            var result = res.Reply.Result;

            Assert.True(result.Count == 5);

            var timeout = result["timeout"];
            var session = result["session-timeout"];
            var mailbox= result["mailbox-size"];
            var procs = result["processes"];
            var strats = result["strategies"];

            Assert.True(timeout.Name == "timeout");
            Assert.True(timeout.Values.Count == 1);
            Assert.True(timeout.Values["value"].Type.Tag == ArgumentTypeTag.Time);
            Assert.True((Time)timeout.Values["value"].Value == 30*seconds);

            Assert.True(session.Name == "session-timeout");
            Assert.True(session.Values.Count == 1);
            Assert.True(session.Values["value"].Type.Tag == ArgumentTypeTag.Time);
            Assert.True((Time)session.Values["value"].Value == 60 * seconds);

            Assert.True(mailbox.Name == "mailbox-size");
            Assert.True(mailbox.Values.Count == 1);
            Assert.True(mailbox.Values["value"].Type.Tag == ArgumentTypeTag.Int);
            Assert.True((int)mailbox.Values["value"].Value == 10000);

            Assert.True(strats.Name == "strategies");
            Assert.True(strats.Values.Count == 1);
            var stratsValue = strats.Values["value"];
            Assert.True(stratsValue.Type.Tag == ArgumentTypeTag.Map);
            Assert.True(stratsValue.Type.GenericType.Tag == ArgumentTypeTag.Strategy);

            Map<string, StrategySettings> ss = (Map<string, StrategySettings>)stratsValue.Value;

            Assert.True(procs.Name == "processes");
            Assert.True(procs.Values.Count == 1);
            var procsValue = procs.Values["value"];
            Assert.True(procsValue.Type.Tag == ArgumentTypeTag.Array);
            Assert.True(procsValue.Type.GenericType.Tag == ArgumentTypeTag.Process);

            Lst<ProcessSettings> ps = (Lst<ProcessSettings>)procsValue.Value;

        }
    }
}

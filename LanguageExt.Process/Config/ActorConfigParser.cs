﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using LanguageExt;
//using LanguageExt.Parsec;
//using static LanguageExt.Prelude;
//using static LanguageExt.Parsec.Prim;
//using static LanguageExt.Parsec.Char;
//using static LanguageExt.Parsec.Expr;
//using static LanguageExt.Parsec.Token;

//namespace LanguageExt
//{
//    public static class ActorConfigParser
//    {
//        // Process config definition
//        readonly static GenLanguageDef definition = GenLanguageDef.Empty.With(
//            CommentStart:   "/*",
//            CommentEnd:     "*/",
//            CommentLine:    "//",
//            NestedComments: true,
//            IdentStart:     letter,
//            IdentLetter:    either(alphaNum, oneOf("-_")),
//            ReservedNames:  List("pid", "strategy", "flags", "mailbox-size", "one-for-one", "all-for-one", "settings"));

//        // Token parser
//        readonly static GenTokenParser tokenParser = 
//            Token.makeTokenParser(definition);

//        // Elements of the token parser to use below
//        readonly static Parser<string> identifier            =  tokenParser.Identifier;
//        readonly static Parser<string> stringLiteral         =  tokenParser.StringLiteral;
//        readonly static Parser<int> integer                  =  tokenParser.Integer;
//        readonly static Parser<int> natural                  =  tokenParser.Natural;
//        readonly static Parser<Unit> whiteSpace              =  tokenParser.WhiteSpace;
//        readonly static Func<string,Parser<string>> symbol   =  tokenParser.Symbol;
//        readonly static Func<string,Parser<string>> reserved =  tokenParser.Reserved;
//        static Parser<T> token<T>(Parser<T> p)               => tokenParser.Lexeme(p);
//        static Parser<T> brackets<T>(Parser<T> p)            => tokenParser.Brackets(p);
//        static Parser<Lst<T>> commaSep<T>(Parser<T> p)       => tokenParser.CommaSep(p);
//        static Parser<Lst<T>> commaSep1<T>(Parser<T> p)      => tokenParser.CommaSep1(p);

//        readonly static Parser<ProcessId> processId =
//            token(
//                from xs in many1(choice(lower, digit, oneOf("@/[,]: ")))
//                let  r   = (new string (xs.ToArray())).Trim()
//                let  pid = ProcessId.TryParse(r)
//                from res in pid.Match(
//                    Right: x  => result(x),
//                    Left:  ex => failure<ProcessId>(ex.Message))
//                select res);

//        readonly static Parser<ProcessConfigToken> pid =
//            from _   in attempt(reserved("pid"))
//            from __  in symbol(":")
//            from pid in processId
//            select new PidToken(pid) as ProcessConfigToken;

//        static Parser<ProcessFlags> flagMap(string name, ProcessFlags flag) =>
//            attempt(
//             from x in symbol(name)
//             select flag);

//        readonly static Parser<ProcessFlags> flag =
//            choice(
//                flagMap("default", ProcessFlags.Default),
//                flagMap("listen-remote-and-local", ProcessFlags.ListenRemoteAndLocal),
//                flagMap("persist-all", ProcessFlags.PersistAll),
//                flagMap("persist-inbox", ProcessFlags.PersistInbox),
//                flagMap("persist-state", ProcessFlags.PersistState),
//                flagMap("remote-publish", ProcessFlags.RemotePublish),
//                flagMap("remote-state-publish", ProcessFlags.RemoteStatePublish));

//        readonly static Parser<ProcessConfigToken> flags =
//            from _  in attempt(reserved("flags"))
//            from __ in symbol(":")
//            from fs in brackets(commaSep(flag))
//            select new FlagsToken(List.fold(fs, ProcessFlags.Default, (s, x) => s | x)) as ProcessConfigToken;

//        readonly static Parser<ProcessConfigToken> maxMailboxSize =
//            from _  in attempt(reserved("mailbox-size"))
//            from __ in symbol(":")
//            from sz in natural
//            select new MailboxSizeToken(sz) as ProcessConfigToken;

//        static Parser<Attr> numericAttr(string name) =>
//            from x in symbol(name)
//            from _ in symbol("=")
//            from v in integer
//            select new NumericAttr(name, v) as Attr;

//        static Parser<Attr> stringAttr(string name) =>
//            from x in symbol(name)
//            from _ in symbol("=")
//            from s in stringLiteral
//            select new StringAttr(name, s) as Attr;

//        static readonly Parser<MessageDirective> fwdToSelf =
//            from _ in reserved("forward-to-self")
//            select new ForwardToSelf() as MessageDirective;

//        static readonly Parser<MessageDirective> fwdToParent =
//            from _ in reserved("forward-to-parent")
//            select new ForwardToParent() as MessageDirective;

//        static readonly Parser<MessageDirective> fwdToDeadLetters =
//            from _ in reserved("forward-to-dead-letters")
//            select new ForwardToDeadLetters() as MessageDirective;

//        static readonly Parser<MessageDirective> stayInQueue =
//            from _ in reserved("stay-in-queue")
//            select new StayInQueue() as MessageDirective;

//        static readonly Parser<MessageDirective> fwdToProcess =
//            from _   in reserved("forward-to-process")
//            from pid in processId
//            select new ForwardToProcess(pid) as MessageDirective;

//        static Parser<MessageDirective> msgDirective =>
//            choice(
//                fwdToDeadLetters,
//                fwdToSelf,
//                fwdToParent,
//                fwdToProcess,
//                stayInQueue);

//        static Parser<Directive> directive =>
//            choice(
//                reserved("resume").Map(_ => Directive.Resume),
//                reserved("restart").Map(_ => Directive.Restart),
//                reserved("stop").Map(_ => Directive.Stop),
//                reserved("escalate").Map(_ => Directive.Escalate));

//        static readonly Parser<string> timeUnit =
//            choice(
//                attempt(reserved("seconds")),
//                attempt(reserved("second")),
//                attempt(reserved("secs")),
//                attempt(reserved("sec")),
//                attempt(reserved("s")),
//                attempt(reserved("minutes")),
//                attempt(reserved("minute")),
//                attempt(reserved("mins")),
//                attempt(reserved("min")),
//                attempt(reserved("milliseconds")),
//                attempt(reserved("millisecond")),
//                attempt(reserved("ms")),
//                attempt(reserved("hours")),
//                attempt(reserved("hour")),
//                reserved("hr"))
//               .label("Unit of time (e.g. seconds, mins, hours, hr, sec, min...)");

//        static Parser<Attr> timeAttr(string name) =>
//            from x in reserved(name)
//            from _ in symbol("=")
//            from v in integer
//            from u in timeUnit
//            select new TimeAttr(name, v, u) as Attr;

//        static Parser<List<Attr>> stratAttrs(string name, params Parser<Attr>[] attrs) =>
//            from n in attempt(reserved(name))
//            from o in symbol("(")
//            from a in commaSep1(choice(attrs.Map(token).Map(attempt)))
//            from c in symbol(")")
//            select a.ToList();

//        readonly static Parser<State<StrategyContext, Unit>> backoff =
//            from attrs in stratAttrs("back-off", timeAttr("min"), timeAttr("max"), timeAttr("step"), timeAttr("duration"))
//            select attrs.Count == 1
//                ? Strategy.Backoff(attrs.GetTimeAttr("duration"))
//                : Strategy.Backoff(attrs.GetTimeAttr("min"), attrs.GetTimeAttr("max"), attrs.GetTimeAttr("step"));

//        readonly static Parser<State<StrategyContext, Unit>> pause =
//            from attrs in stratAttrs("pause", timeAttr("duration"))
//            select Strategy.Pause(attrs.GetTimeAttr("duration"));

//        readonly static Parser<State<StrategyContext, Unit>> always =
//            from n in attempt(reserved("always"))
//            from _ in symbol(":")
//            from d in token(directive)
//            select Strategy.Always(d);

//        readonly static Parser<Type> type =
//            from x in letter
//            from xs in many1(choice(letter, ch('.'), ch('_')))
//            select Type.GetType(new string(x.Cons(xs).ToArray()));

//        readonly static Parser<State<Exception, Option<Directive>>> exceptionDirective =
//            from b in symbol("|")
//            from t in token(type)
//            from a in symbol("->")
//            from d in token(directive)
//            select Strategy.With(d, t);

//        readonly static Parser<State<Exception, Option<Directive>>> otherwiseDirective =
//            from b in symbol("|")
//            from t in symbol("_")
//            from a in symbol("->")
//            from d in token(directive)
//            select Strategy.Otherwise(d);

//        readonly static Parser<State<Directive, Option<MessageDirective>>> matchMessageDirective =
//            from b in symbol("|")
//            from d in token(directive)
//            from a in symbol("->")
//            from m in token(msgDirective)
//            select Strategy.When(m, d);

//        readonly static Parser<State<Directive, Option<MessageDirective>>> otherwiseMsgDirective =
//            from b in symbol("|")
//            from t in symbol("_")
//            from a in symbol("->")
//            from d in token(msgDirective)
//            select Strategy.Otherwise(d);

//        readonly static Parser<State<StrategyContext, Unit>> match =
//            from _      in attempt(reserved("match"))
//            from direx  in many(attempt(exceptionDirective))
//            from other  in optional(otherwiseDirective)
//            let dirs = direx.Append(other.AsEnumerable()).ToArray()
//            from ok     in dirs.Length > 0
//                ? result(dirs)
//                : failure<State<Exception, Option<Directive>>[]>("'match' must be followed by at least one clause")
//            select Strategy.Match(dirs);

//        readonly static Parser<State<StrategyContext, Unit>> redirectMatch =
//            from direx in many(attempt(matchMessageDirective))
//            from other in optional(otherwiseMsgDirective)
//            let dirs = direx.Append(other.AsEnumerable()).ToArray()
//            from ok in dirs.Length > 0
//                ? result(dirs)
//                : failure<State<Directive, Option<MessageDirective>>[]>("'redirect when' must be followed by at least one clause")
//            select Strategy.Redirect(dirs);

//        readonly static Parser<State<StrategyContext, Unit>> redirect =
//            from n in attempt(reserved("redirect"))
//            from t in either(attempt(symbol(":")), reserved("when"))
//            from r in t == ":"
//               ? from d in token(msgDirective)
//                 select Strategy.Redirect(d)
//               : redirectMatch
//            select r;

//        readonly static Parser<State<StrategyContext, Unit>> retries =
//            from attrs in stratAttrs("retries", numericAttr("count"), timeAttr("duration"))
//            select attrs.Count == 1
//                ? Strategy.Retries(attrs.GetNumericAttr("count"))
//                : Strategy.Retries(attrs.GetNumericAttr("count"), attrs.GetTimeAttr("duration"));

//        readonly static Parser<Tuple<string, string>> setting =
//            from key in identifier
//            from _   in symbol(":")
//            from val in stringLiteral
//            select Tuple(key, val);

//        readonly static Parser<ProcessConfigToken> settings =
//            from n in attempt(reserved("settings"))
//            from _ in symbol(":")
//            from s in many1(attempt(setting))
//            select new SettingsToken(Map.createRange(s)) as ProcessConfigToken;

//        readonly static Parser<IEnumerable<State<StrategyContext, Unit>>> strategies =
//            many1(
//                choice(
//                    retries,
//                    backoff,
//                    always,
//                    redirect,
//                    match));

//        readonly static Parser<State<StrategyContext, Unit>> oneForOne =
//            from a in attempt(reserved("one-for-one"))
//            from b in symbol(":")
//            from attrs in strategies
//            select Strategy.OneForOne(attrs.ToArray());

//        readonly static Parser<State<StrategyContext, Unit>> allForOne =
//            from a in attempt(reserved("all-for-one"))
//            from b in symbol(":")
//            from attrs in strategies
//            select Strategy.AllForOne(attrs.ToArray());

//        readonly static Parser<ProcessConfigToken> strategy =
//            from a in attempt(reserved("strategy"))
//            from b in symbol(":")
//            from s in either(attempt(oneForOne), allForOne)
//            select new StrategyToken(s) as ProcessConfigToken;

//        public readonly static Parser<ActorConfig> Parser =
//            from _ in whiteSpace
//            from tokens in many1(
//                choice(
//                    pid,
//                    flags,
//                    strategy,
//                    settings,
//                    maxMailboxSize))
//            select new ActorConfig(tokens);
//    }
//}

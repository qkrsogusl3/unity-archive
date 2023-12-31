using System;
using Cysharp.Threading.Tasks;

namespace UniBloc.Samples.Counter
{
    public class CounterBloc : PooledBloc<CounterEvent, int>
    {
        public CounterBloc(ConcurrencyMode mode) : base(0)
        {
            On<CounterEvent.Decrement>(async (e, emitter, token) =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
                emitter.Emit(State - 1);
            }, mode);
            On<CounterEvent.Increment>((e, emitter) =>
            {
                if (State >= 10)
                {
                    AddError(new Exception($"overflow error: {State}"));
                    return;
                }

                emitter.Emit(State + 1);
            });
            On<CounterEvent.Reset>((e, emitter) => emitter.Emit(0));
        }

        protected override void OnError(Exception error)
        {
            Add<CounterEvent.Reset>();
        }
    }

    public class CounterEvent : EventBase<CounterEvent>
    {
        public sealed class Increment : CounterEvent
        {
            public override string ToString() => nameof(Increment);
        }

        public sealed class Decrement : CounterEvent
        {
            public override string ToString() => nameof(Decrement);
        }

        public sealed class Reset : CounterEvent
        {
            public override string ToString() => nameof(Reset);
        }
    }
}
using System;
using Circle.Game.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">CS0453</typeparam>
    public class RollingItem<T> : Component
    {
        /// <summary>
        /// 값을 올바르게 할당해야합니다. 값이 같은 아이템이 있으면 올바른 작동을 하지 않을 수 있습니다.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 해당 아이템이 선택되었을 때 실행됩니다.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// 임의로 값을 표시할 수 있습니다.
        /// </summary>
        public string Text;

        [Resolved]
        private CircleConfigManager localConfig { get; set; }

        [Resolved]
        private FrameworkConfigManager config { get; set; }

        public RollingItem(T value, Action action = null, string text = null)
        {
            Value = value;
            Action = action;
            Text = text ?? value.ToString();
        }

        public RollingItem(CircleSetting lookup, T value, string text = null)
        {
            Value = value;
            Action = () => localConfig.SetValue(lookup, value);
            Text = text ?? value.ToString();
        }

        public RollingItem(FrameworkSetting lookup, T value, string text = null)
        {
            Value = value;
            Action = () => config.SetValue(lookup, value);
            Text = text ?? value.ToString();
        }
    }
}

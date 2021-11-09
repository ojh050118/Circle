using System;
using Circle.Game.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Configuration;

namespace Circle.Game.Graphics.UserInterface
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">CS0453</typeparam>
    public class RollingItem<T>
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

        /// <summary>
        /// 값과 액션을 포함하는 아이템.
        /// </summary>
        /// <param name="value">값.</param>
        /// <param name="action"></param>
        [Obsolete]
        public RollingItem(T value, Action action = null)
        {
            Value = value;
            Action = action;
            Text = Value.ToString();
        }

        /// <summary>
        /// 값과 액션을 포함하는 아이템.
        /// </summary>
        /// <param name="value">값.</param>
        /// <param name="text">임의로 표시할 내용</param>
        /// <param name="action"></param>
        [Obsolete]
        public RollingItem(T value, string text, Action action = null)
        {
            Value = value;
            Text = text;
            Action = action;
        }

        public RollingItem(T value, CircleSetting lookup, string text = null)
        {
            Value = value;
            Action = () => localConfig.SetValue(lookup, value);
            Text = text != null ? text : value.ToString();
        }

        public RollingItem(T value, FrameworkSetting lookup, string text = null)
        {
            Value = value;
            Action = () => config.SetValue(lookup, value);
            Text = text != null ? text : value.ToString();
        }
    }
}

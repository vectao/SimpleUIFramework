using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{

    public class ButtonEx : Button
    {

        [SerializeField, FormerlySerializedAs("onPress")]
        private ButtonClickedEvent m_OnPress = new ButtonClickedEvent();

		[SerializeField, HideInInspector]
		public int m_SfxId = 3000011;

        private int m_LongPressState = 0;
        private int m_LongPressFrame = 0;

        public ButtonClickedEvent onPress
        {
            get { return m_OnPress; }
            set { m_OnPress = value; }
        }

        [SerializeField, FormerlySerializedAs("onRelease")]
        private ButtonClickedEvent m_OnRelease = new ButtonClickedEvent();

        public ButtonClickedEvent onRelease
        {
            get { return m_OnRelease; }
            set { m_OnRelease = value; }
        }

        [SerializeField, FormerlySerializedAs("onExit")]
        private ButtonClickedEvent m_OnExit = new ButtonClickedEvent();

        public ButtonClickedEvent onExit
        {
            get { return m_OnExit; }
            set { m_OnExit = value; }
        }

        /// <summary>
        /// 暂时Long Press有点问题
        /// </summary>
        [SerializeField, FormerlySerializedAs("onLongPress")]
        private ButtonClickedEvent m_OnLongPress = new ButtonClickedEvent();

        public ButtonClickedEvent onLongPress
        {
            get { return m_OnLongPress; }
            set { m_OnLongPress = value; }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            CancelInvoke("LongPressTimer");
            Invoke("LongPressTimer", 1f);
            m_LongPressState = 1;
            m_OnPress.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            m_LongPressState = m_LongPressState == 2 ? 3 : -1;
            CancelInvoke("LongPressTimer");
            base.OnPointerExit(eventData);
            m_OnExit.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            CancelInvoke("LongPressTimer");
            m_OnRelease.Invoke();
            m_LongPressFrame = m_LongPressState > 1 ? Time.frameCount : 0;
        }
        /*
        public override bool IsInteractable()
        {
            return m_LongPressFrame != Time.frameCount && base.IsInteractable();
        }
        */

        private void LongPressTimer()
        {
            if (m_LongPressState == 1)
            {
                m_LongPressState = 2;
                //m_OnLongPress.Invoke();
            }
        }

    }

}


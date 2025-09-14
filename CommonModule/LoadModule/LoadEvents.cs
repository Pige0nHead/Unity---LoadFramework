using Framework;

namespace LoadFramework {
    /// <summary>
    /// �㲥������Ϣ�õ��¼�
    /// </summary>
    public class LoadingEvent : Event<LoadingEvent, ILoadEventInfo> { }
    /// <summary>
    /// ��������ʼ���ص�ʱ��㲥����¼�������¼������������Ƽ���UI����ʾ��
    /// </summary>
    public class StartLoadingEvent : Event<StartLoadingEvent> { }
    /// <summary>
    /// ������ȫ���������֮��㲥����¼�������¼������������Ƽ���UI�Ĺرգ�
    /// </summary>
    public class LoadingCompletedEvent : Event<LoadingCompletedEvent> { }
    /// <summary>
    /// ����������ǰ���ؽ��ȵ��¼���������Ҫ���Զ�������¼������¼��ؽ���UI
    /// </summary>
    public class CurrentLoadingStepEvent : Event<CurrentLoadingStepEvent, string> { }
}
using UnityEngine;

namespace OptimazeScroll
{
    /// <summary>
    /// ���������������� ������.
    /// </summary>
    public interface IScrollInitializable
    {
        /// <summary>
        /// ���������������� ����� � �������.
        /// </summary>
        /// <param name="objectForInitialize">������, ������� ����� ���������������� ���������������� ������.</param>
        void InScrollInitialize(IScrollInitializeObject objectForInitialize);
        /// <summary>
        /// ������� ����� ������ �� ������ ��������������� � �������������� Instantiate.
        /// </summary>
        IScrollInitializable Clone(Transform parent);
        /// <summary>
        /// �������� RectTransform ����� �������.
        /// </summary>
        /// <returns></returns>
        RectTransform GetRectTransform();
    }
}
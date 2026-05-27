using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fullscreen ON/OFF: một click đổi mode; nút sáng = trạng thái hiện tại.
/// </summary>
public static class FullscreenUiHelper
{
    private const int FullScreenWidth = 1920;
    private const int FullScreenHeight = 1080;
    private const int WindowedWidth = 1280;
    private const int WindowedHeight = 720;

    private static readonly Color ActiveBackground = Color.white;
    private static readonly Color InactiveBackground = new Color(0.35f, 0.35f, 0.35f, 1f);
    private static readonly Color ActiveLabel = Color.black;
    private static readonly Color InactiveLabel = new Color(0.5f, 0.5f, 0.5f, 1f);

    public static bool IsFullScreenActive()
    {
        return Screen.fullScreenMode != FullScreenMode.Windowed;
    }

    public static void ApplyDisplayMode(bool enabled)
    {
        if (enabled)
        {
            Screen.SetResolution(FullScreenWidth, FullScreenHeight, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(WindowedWidth, WindowedHeight, FullScreenMode.Windowed);
        }
    }

    /// <param name="forceFullScreen">
    /// Trạng thái vừa chọn (khi click). Null = đọc từ Screen (khi mở panel).
    /// </param>
    public static void RefreshButtons(Button onButton, Button offButton, bool? forceFullScreen = null)
    {
        bool isFullScreen = forceFullScreen ?? IsFullScreenActive();

        if (onButton != null)
        {
            onButton.interactable = true;
            SetButtonActiveLook(onButton, isFullScreen);
        }

        if (offButton != null)
        {
            offButton.interactable = true;
            SetButtonActiveLook(offButton, !isFullScreen);
        }
    }

    private static void SetButtonActiveLook(Button button, bool active)
    {
        Color background = active ? ActiveBackground : InactiveBackground;
        Color labelColor = active ? ActiveLabel : InactiveLabel;

        Graphic graphic = button.targetGraphic;
        if (graphic != null)
        {
            graphic.color = background;
        }

        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.color = labelColor;
        }

        // Color Tint ghi đè graphic.color sau click — đồng bộ cả ColorBlock.
        if (button.transition == Selectable.Transition.ColorTint)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = background;
            colors.highlightedColor = background;
            colors.pressedColor = background;
            colors.selectedColor = background;
            button.colors = colors;
        }
    }
}

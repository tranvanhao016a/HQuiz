﻿using BlazorQuiz.Shared;

namespace BlazorQuiz.Shared;

public class AppState : IAppState
{
    public string? LoadingText { get; private set; }

    public event Action? OnToggleLoader;

    public void HideLoader()
    {
        LoadingText = null;
        OnToggleLoader?.Invoke();
    }

    public void ShowLoader(string loadingText)
    {

        LoadingText = loadingText;
        OnToggleLoader?.Invoke();
    }
}

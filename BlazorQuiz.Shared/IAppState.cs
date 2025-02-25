using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorQuiz.Shared
{
    public interface IAppState
    {
        string? LoadingText { get; }
        void ShowLoader(string loadingText);
    
        void HideLoader();

        event Action? OnToggleLoader;
    }
}

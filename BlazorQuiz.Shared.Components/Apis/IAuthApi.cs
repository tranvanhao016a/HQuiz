﻿using BlazorQuiz.Shared.DTO;
using Refit;
namespace BlazorQuiz.Web.Apis
{
    [Headers("Authorization: Bearer")]
    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        [Post("/api/auth/register")]
        Task<QuizApiResponse> RegisterAsync(RegisterDto dto);
    }

}

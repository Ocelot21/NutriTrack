﻿using System.Net.Http.Json;
using NutriTrack.Api.Contracts.Notifications;

namespace NutriTrack.Api.Common.Clients;

public sealed class NotificationsClient
{
    private readonly HttpClient _http;

    public NotificationsClient(HttpClient http) => _http = http;

    public async Task<string> GetForUserRaw(
        Guid userId,
        bool unreadOnly,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var url = $"internal/users/{userId}/notifications?onlyUnread={unreadOnly}&page={page}&pageSize={pageSize}";
        return await _http.GetStringAsync(url, cancellationToken);
    }

    public async Task<int> GetUnreadCount(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var url = $"internal/users/{userId}/notifications/unread-count";
        var response = await _http.GetFromJsonAsync<UnreadCountResponse>(url, cancellationToken);

        return response?.Count ?? 0;
    }

    public async Task MarkRead(
        Guid userId,
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var url = $"internal/users/{userId}/notifications/{notificationId}/read";
        var response = await _http.PostAsync(url, null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task MarkAllRead(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var url = $"internal/users/{userId}/notifications/mark-all-read";
        var response = await _http.PostAsync(url, null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}


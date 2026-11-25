using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;

namespace LibraryHub.Web.Services
{
    public class LibraryApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _http;

        public LibraryApiClient(HttpClient http)
        {
            _http = http;
        }

        // Dashboard
        public Task<ApiResponse<DashboardDto>> GetDashboardAsync() =>
            GetAsync<DashboardDto>("api/dashboard");

        // Books
        public Task<ApiResponse<PagedResult<BookDto>>> GetBooksAsync(
            string? search, int page, int pageSize, bool availableOnly = false)
        {
            var url = $"api/books?search={Uri.EscapeDataString(search ?? string.Empty)}" +
                      $"&page={page}&pageSize={pageSize}&availableOnly={availableOnly}";
            return GetAsync<PagedResult<BookDto>>(url);
        }

        public Task<ApiResponse<BookDto>> GetBookAsync(int id) =>
            GetAsync<BookDto>($"api/books/{id}");

        public Task<ApiResponse<BookDto>> CreateBookAsync(SaveBookDto dto) =>
            SendAsync<BookDto>(HttpMethod.Post, "api/books", dto);

        public Task<ApiResponse<BookDto>> UpdateBookAsync(int id, SaveBookDto dto) =>
            SendAsync<BookDto>(HttpMethod.Put, $"api/books/{id}", dto);

        public Task<ApiResponse<bool>> DeleteBookAsync(int id) =>
            SendAsync<bool>(HttpMethod.Delete, $"api/books/{id}");

        // Members
        public Task<ApiResponse<PagedResult<MemberDto>>> GetMembersAsync(
            string? search, int page, int pageSize, bool activeOnly = false)
        {
            var url = $"api/members?search={Uri.EscapeDataString(search ?? string.Empty)}" +
                      $"&page={page}&pageSize={pageSize}&activeOnly={activeOnly}";
            return GetAsync<PagedResult<MemberDto>>(url);
        }

        public Task<ApiResponse<MemberDto>> GetMemberAsync(int id) =>
            GetAsync<MemberDto>($"api/members/{id}");

        public Task<ApiResponse<MemberDto>> CreateMemberAsync(SaveMemberDto dto) =>
            SendAsync<MemberDto>(HttpMethod.Post, "api/members", dto);

        public Task<ApiResponse<MemberDto>> UpdateMemberAsync(int id, SaveMemberDto dto) =>
            SendAsync<MemberDto>(HttpMethod.Put, $"api/members/{id}", dto);

        public Task<ApiResponse<bool>> DeleteMemberAsync(int id) =>
            SendAsync<bool>(HttpMethod.Delete, $"api/members/{id}");

        // Loans
        public Task<ApiResponse<List<LoanDto>>> GetLoansAsync(string status) =>
            GetAsync<List<LoanDto>>($"api/loans?status={Uri.EscapeDataString(status)}");

        public Task<ApiResponse<LoanDto>> BorrowAsync(CreateLoanDto dto) =>
            SendAsync<LoanDto>(HttpMethod.Post, "api/loans", dto);

        public Task<ApiResponse<LoanDto>> ReturnLoanAsync(int id) =>
            SendAsync<LoanDto>(HttpMethod.Post, $"api/loans/{id}/return");

        // Helpers
        private async Task<ApiResponse<T>> GetAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);
            return await ReadAsync<T>(response);
        }

        private async Task<ApiResponse<T>> SendAsync<T>(HttpMethod method, string url, object? body = null)
        {
            using var request = new HttpRequestMessage(method, url);

            if (body != null)
            {
                request.Content = JsonContent.Create(body, options: JsonOptions);
            }

            var response = await _http.SendAsync(request);
            return await ReadAsync<T>(response);
        }

        private static async Task<ApiResponse<T>> ReadAsync<T>(HttpResponseMessage response)
        {
            // Unexpected server errors are not handled on the page; they go to the error page
            if ((int)response.StatusCode >= 500)
            {
                throw new HttpRequestException("The API returned an unexpected error.");
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new ApiResponse<T> { Success = true };
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions);
            return result ?? ApiResponse<T>.Fail("The API returned an empty response.");
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Interfaces;
using System.Linq;
using System;

namespace ProductivityKeeperWeb.Services
{
    public class DiaryService
    {
        private readonly IDiaryRepository _repository;
        private readonly IAuthService _authService;
        private readonly int _unitId;
        public DiaryService(IDiaryRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;

            _unitId = _authService.GetUnitId();
        }

        public async Task<IEnumerable<Domain.DTO.DiaryPreviewItemDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync(_unitId);
            return items.Select(x => new Domain.DTO.DiaryPreviewItemDto {
                Id = x.Id,
                Title = x.Title,
                ContentPreview = GetContentPreview(x.Content),
                ImageUrl = GetFirstImageUrl(x.Content),
                UpdatedAt = x.UpdatedAt
            });
        }

        private string? GetFirstImageUrl(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;
            var match = System.Text.RegularExpressions.Regex.Match(content, "<img[^>]*src=[\"']([^\"'>]+)[\"'][^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        private string? GetContentPreview(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;
            // Remove all <img ...> tags
            var noImages = System.Text.RegularExpressions.Regex.Replace(content, "<img[^>]*>", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Take first 3000 characters
            return noImages.Length > 300 ? noImages.Substring(0, 300) : noImages;
        }

        public async Task<Domain.DTO.DiaryItemDto> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id, _unitId);
            if (item == null) return null;
            return new Domain.DTO.DiaryItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Content = item.Content,
                UpdatedAt = item.UpdatedAt
            };
        }

        public async Task<Domain.DTO.DiaryItemDto> AddAsync(Domain.DTO.DiaryItemDto dto)
        {
            var item = new Domain.Models.DiaryItem
            {
                Title = dto.Title,
                Content = dto.Content,
                UpdatedAt = DateTime.UtcNow,
                UnitId = _unitId
            };
            var created = await _repository.AddAsync(item);
            return new Domain.DTO.DiaryItemDto
            {
                Id = created.Id,
                Title = created.Title,
                Content = created.Content,
                UpdatedAt = created.UpdatedAt
            };
        }

        public async Task<bool> UpdateAsync(int id, Domain.DTO.DiaryItemDto dto)
        {
            var item = await _repository.GetByIdAsync(id, _unitId);
            if (item == null) return false;
            item.Title = dto.Title;
            item.Content = dto.Content;
            item.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(item);
            return true;
        }

        public Task DeleteAsync(int id) => _repository.DeleteAsync(id, _unitId);
    }
}

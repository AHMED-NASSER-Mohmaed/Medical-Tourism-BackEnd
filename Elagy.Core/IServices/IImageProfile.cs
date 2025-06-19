using Elagy.Core.DTOs.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public  interface IImageProfile
    {
        Task<FileUploadResponseDto> UpdateImageProfile(string _user_id,IFormFile _);
        Task<FileDeletionResponseDto> DeleteImageProfile(string _userId);

    }
}

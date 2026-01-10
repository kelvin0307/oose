using Core.Common;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Services.Abstractions;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;

public class MaterialService : Generatable<Material>, IMaterialService
{
    private readonly IRepository<Material> materialRepository;

    public MaterialService(IDocumentFactory documentFactory, IRepository<Material> materialRepository) : base(documentFactory)
    {
        this.materialRepository = materialRepository;
    }

    public override async Task<Response<DocumentDTO>> GenerateDocument(int materialId, DocumentTypes documentType)
    {
        try
        {
            var material = await materialRepository.Get(materialId);

            if (material == null)
            {
                return Response<DocumentDTO>.Fail("Error generating material document");
            }

            var documentData = MapToDocumentDataDTO(material);

            return Response<DocumentDTO>.Ok(CreateDocument(documentData, documentType));
        }
        catch (Exception ex)
        {
            return Response<DocumentDTO>.Fail("Error generating material document" + ex.Message);
        }
    }

    protected override DocumentDataDTO MapToDocumentDataDTO(Material material)
    {
        var dict = new Dictionary<string, string>
        {
            { "", material.Content }
        };

        return new DocumentDataDTO()
        {
            Title = material.Name,
            Paragraphs = dict
        };
    }
}

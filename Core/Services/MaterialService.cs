using AutoMapper;
using Core.Common;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Services.Abstractions;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;

public class MaterialService : Generatable<Material, MaterialIdDTO>, IMaterialService
{
    private readonly IRepository<Material> materialRepository;
    private readonly IRepository<Lesson> lessonRepository;
    private readonly IMapper mapper;

    public MaterialService(IDocumentFactory documentFactory, IRepository<Material> materialRepository, IRepository<Lesson> lessonRepository, IMapper mapper) : base(documentFactory)
    {
        this.materialRepository = materialRepository;
        this.lessonRepository = lessonRepository;
        this.mapper = mapper;
    }

    public override Task<Response<DocumentDTO>> GenerateDocument(MaterialIdDTO materialId, DocumentTypes documentType)
    {
        try
        {
            var material = materialRepository.Find(x => x.Id == materialId.MaterialId && x.Version == materialId.Version).FirstOrDefault();

            if (material == null)
            {
                return Task.FromResult(Response<DocumentDTO>.Fail("Error generating material document"));
            }

            var documentData = MapToDocumentDataDTO(material);

            return Task.FromResult(Response<DocumentDTO>.Ok(CreateDocument(documentData, documentType)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Response<DocumentDTO>.Fail("Error generating material document" + ex.Message));
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

    public async Task<Response<MaterialDTO>> UpdateMaterial(UpdateMaterialDTO updatedMaterial)
    {
        try
        {
            var currentMaterial = materialRepository.Find(x => x.Id == updatedMaterial.Id && x.Version == updatedMaterial.Version).FirstOrDefault();

            if (currentMaterial == null)
            {
                return Response<MaterialDTO>.Fail("Material not found");
            }

            var newMaterial = new Material
            {
                Id = currentMaterial.Id,
                Name = updatedMaterial.Name,
                Content = updatedMaterial.Content,
                Version = currentMaterial.Version + 1,
                LessonId = currentMaterial.LessonId
            };


            await materialRepository.CreateAndCommit(newMaterial);

            return Response<MaterialDTO>.Ok(mapper.Map<MaterialDTO>(newMaterial));

        }
        catch (Exception ex)
        {
            return Response<MaterialDTO>.Fail("Error updating material: " + ex.Message);
        }
    }
    public async Task<Response<MaterialDTO>> CreateMaterial(CreateMaterialDTO createMaterialDTO)
    {
        try
        {
            var lesson = await lessonRepository.Get(createMaterialDTO.LessonId);

            if (lesson == null)
            {
                return Response<MaterialDTO>.Fail("Lesson not found");
            }
            var allMaterials = await materialRepository.GetAll();
            var newId = allMaterials.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();

            var newMaterial = new Material
            {
                Id = newId + 1,
                Name = createMaterialDTO.Name,
                Content = createMaterialDTO.Content,
                Version = 1,
                LessonId = lesson.Id
            };

            await materialRepository.CreateAndCommit(newMaterial);

            return Response<MaterialDTO>.Ok(mapper.Map<MaterialDTO>(newMaterial));

        }
        catch (Exception ex)
        {
            return Response<MaterialDTO>.Fail("Error creating material: " + ex.Message);
        }
    }
    public async Task<Response<IList<MaterialDTO>>> GetMaterialByLessonId(int lessonId)
    {
        try
        {
            var materials = await materialRepository.GetAll(x => x.LessonId == lessonId && x.SysDeleted == null);

            var latestMaterials = materials
            .GroupBy(m => m.Id)
            .Select(g => g.OrderByDescending(m => m.Version).First())
            .ToList();

            return Response<IList<MaterialDTO>>.Ok(mapper.Map<IList<MaterialDTO>>(latestMaterials));
        }
        catch (Exception ex)
        {
            return Response<IList<MaterialDTO>>.Fail("Error retrieving material: " + ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteMaterial(int materialId)
    {
        try
        {
            var materials = await materialRepository.GetAll(x => x.Id == materialId);

            foreach (var material in materials)
            {
                material.SysDeleted = DateTimeOffset.UtcNow;
                materialRepository.Update(material);
            }

            await materialRepository.SaveManually();

            return Response<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Response<bool>.Fail("Error retrieving material: " + ex.Message);
        }
    }
}

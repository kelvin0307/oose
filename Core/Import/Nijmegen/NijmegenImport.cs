using Core.Interfaces.Adapters;
using Core.DTOs.Imports.Nijmegen;
using Core.Mappers;
using Domain.Models;

namespace Core.Import.Nijmegen;

public class NijmegenImport: IImportAdapter<NijmegenImportDataDto>
{
    public Course GetMappedCourseData(NijmegenImportDataDto data)
    {
        return NijmegenImportMapper.Map(data);
    }
}
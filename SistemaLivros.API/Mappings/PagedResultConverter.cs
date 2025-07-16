using AutoMapper;
using SistemaLivros.Application.Common;
using System.Collections.Generic;
using System.Linq;

namespace SistemaLivros.API.Mappings
{
    /// <summary>
    /// Conversor personalizado para mapear PagedResult<TSource> para PagedResult<TDestination>
    /// </summary>
    /// <typeparam name="TSource">Tipo de origem</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    public class PagedResultConverter<TSource, TDestination> : ITypeConverter<PagedResult<TSource>, PagedResult<TDestination>>
    {
        private readonly IMapper _mapper;

        public PagedResultConverter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public PagedResult<TDestination> Convert(PagedResult<TSource> source, PagedResult<TDestination> destination, ResolutionContext context)
        {
            // Mapeia cada item da lista de origem para o tipo de destino
            var items = source.Items.Select(item => _mapper.Map<TDestination>(item)).ToList();
            
            // Cria um novo PagedResult com os itens mapeados e os mesmos metadados de paginação
            return new PagedResult<TDestination>
            {
                Items = items,
                PageNumber = source.PageNumber,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount,
                TotalPages = source.TotalPages
            };
        }
    }
}

using PawnCloud.PawnItems.Dto;
using System.Collections.Generic;

namespace PawnCloud.PawnTickets.Dto;

public class CreatePawnTicketWithItemsDto
{
    public CreateOrEditPawnTicketDto Ticket { get; set; } = new CreateOrEditPawnTicketDto();
    public List<CreateOrEditPawnItemDto> Items { get; set; } = new List<CreateOrEditPawnItemDto>();
}
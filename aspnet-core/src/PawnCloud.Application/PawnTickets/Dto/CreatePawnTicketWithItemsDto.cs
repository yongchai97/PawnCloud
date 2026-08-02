using System.Collections.Generic;
using PawnCloud.PawnItems.Dto;

namespace PawnCloud.PawnTickets.Dto;

public class CreatePawnTicketWithItemsDto
{
    public CreateOrEditPawnTicketDto Ticket { get; set; }
    public List<CreateOrEditPawnItemDto> Items { get; set; }
}
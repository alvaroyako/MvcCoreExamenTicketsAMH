using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcCoreExamenTicketsAMH.Models;
using MvcCoreExamenTicketsAMH.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreExamenTicketsAMH.Controllers
{
    public class EmpresaController : Controller
    {
        private ServiceApiEmpresa service;

        public EmpresaController(ServiceApiEmpresa service)
        {
            this.service = service;
        }

        public IActionResult TicketsUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TicketsUsuario(int idusuario)
        {
            List<Ticket> tickets =await this.service.GetTicketsUsuarioAsync(idusuario);
            return View(tickets);
        }

        public IActionResult Ticket()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ticket(int idticket)
        {
            Ticket ticket = await this.service.GetTicketAsync(idticket);
            return View(ticket);
        }

        public IActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(Usuario usu)
        {
            await this.service.CreateUsuarioAsync(usu);
            ViewData["MENSAJE"] = "Usuario creado con exito";
            return View();
        }

        public IActionResult CrearTicket()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearTicket(int idusuario,DateTime fecha,string importe,string producto,IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadBlobAsync("blobsexamen", blobName, stream);

            }

            

            Ticket ticket = new Ticket();
            ticket.IdTicket = 0;
            ticket.IdUsuario = idusuario;
            ticket.Fecha = fecha;
            ticket.Importe = importe;
            ticket.Producto = producto;
            ticket.FileName = file.FileName;
            ticket.StoragePath = "https://storageexamenamh.blob.core.windows.net/blobsexamen/" + file.FileName;
            ViewData["TICKET"] = "ticket creado";
          return View();
        }

        public async Task<IActionResult> ProcessTicket(int idticket, string filename)
        {
            await this.service.ProcessTicketAsync(idticket,filename);
            return RedirectToAction("TicketsUsuario","Empresa");
        }
    }
}

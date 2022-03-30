using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using MvcCoreExamenTicketsAMH.Models;
using Newtonsoft.Json;
using System.Text;
using Azure.Storage.Blobs;
using System.IO;
using Azure.Storage.Blobs.Models;

namespace MvcCoreExamenTicketsAMH.Services
{
    public class ServiceApiEmpresa
    {
        private string UriApi;
        private MediaTypeWithQualityHeaderValue Header;
        private BlobServiceClient client;

        public ServiceApiEmpresa(string url,BlobServiceClient client)
        {
            this.UriApi = url;
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.client = client;
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UriApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Ticket>> GetTicketsUsuarioAsync(int idusuario)
        {
            string request = "/TicketsUsuario/"+idusuario;
            List<Ticket> tickets = await this.CallApiAsync<List<Ticket>>(request);
            return tickets;
        }

        public async Task<Ticket> GetTicketAsync(int idticket)
        {
            string request = "/FindTicket/" + idticket;
            Ticket ticket = await this.CallApiAsync<Ticket>(request);
            return ticket;
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/CreateTicket";
                client.BaseAddress = new Uri(this.UriApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                Ticket t = new Ticket();
                t.IdTicket = ticket.IdTicket;
                t.IdUsuario = ticket.IdUsuario;
                t.Fecha = ticket.Fecha;
                t.Importe = ticket.Importe;
                t.Producto = ticket.Producto;
                t.FileName = ticket.FileName;
                t.StoragePath = ticket.StoragePath;
               
                string json = JsonConvert.SerializeObject(t);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task CreateUsuarioAsync(Usuario usu)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/CreateUsuario";
                client.BaseAddress = new Uri(this.UriApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                Usuario user = new Usuario();
                user.IdUsuario = usu.IdUsuario;
                user.Nombre = usu.Nombre;
                user.Apellidos = usu.Apellidos;
                user.Email = usu.Email;
                user.UserName = usu.UserName;
                user.Password = usu.Password;

                string json = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }
    
        public async Task ProcessTicketAsync(int idticket, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/ProcessTicket";
                client.BaseAddress = new Uri(this.UriApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                Ticket t = new Ticket();
                t.IdTicket = idticket;
                t.IdUsuario = 0;
                t.Fecha = DateTime.Now;
                t.Importe = "";
                t.Producto = "";
                t.FileName = fileName;
                t.StoragePath ="";

                string json = JsonConvert.SerializeObject(t);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        
    }
}

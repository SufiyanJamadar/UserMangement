﻿using AutoMapper;
using LearnApi.Helper;
using LearnApi.Modal;
using LearnApi.Repos;
using LearnApi.Repos.Models;
using LearnApi.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LearnApi.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LearndataContext context;
        private readonly IMapper mapper;
        private readonly ILogger<CustomerService> logger;
        public CustomerService(LearndataContext context, IMapper mapper, ILogger<CustomerService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<APIResponse> Create(Customermodal data)
        {
            APIResponse response = new APIResponse();
            try
            {
                this.logger.LogInformation("Create Begins");
                TblCustomer _customer = this.mapper.Map<Customermodal, TblCustomer>(data);
                await this.context.TblCustomers.AddAsync(_customer);
                await this.context.SaveChangesAsync();

                response.ResponseCode = 201;
                response.Result = "Pass";
                response.Message="Customer Created Successfuly";
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Message= ex.Message;
                this.logger.LogError(ex.Message,ex);
            }
            return response;
        }

        public async  Task<List<Customermodal>> GetAll()
        {
           List<Customermodal> _response=new List<Customermodal>();
           var _data= await this.context.TblCustomers.ToListAsync();
            if(_data is not null)
            {
                _response = this.mapper.Map<List<TblCustomer>,List<Customermodal>>(_data);
            }
            return _response;
        }

        public async Task<Customermodal> GetByCode(string code)
        {
            Customermodal _response = new Customermodal();
            var _data = await this.context.TblCustomers.FindAsync(code);
            if (_data is not null)
            {
                _response = this.mapper.Map<TblCustomer,Customermodal>(_data);
            }
            return _response;
        }

        public async Task<APIResponse> Remove(string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                var _customer = await this.context.TblCustomers.FindAsync(code);
                if( _customer is not null )
                {
                    this.context.TblCustomers.Remove(_customer);
                    await this.context.SaveChangesAsync();
                    response.ResponseCode = 201;
                    response.Result = "Pass";
                    response.Message = "Customer Deleted Successfully";
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Result = "Fail";
                    response.Message = "Data Not Found";
                }
                
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Result = "Fail";
                response.Message = ex.Message;
            }
            return response;
        }

       

        public async Task<APIResponse> Update(Customermodal data, string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                var _customer = await this.context.TblCustomers.FindAsync(code);
                if (_customer is not null)
                {
                    _customer.Name = data.Name;
                    _customer.Email = data.Email;
                    _customer.Phone = data.Phone;
                    _customer.IsActive = data.IsActive;
                    _customer.Creditlimit = data.Creditlimit;
                    await this.context.SaveChangesAsync();
                    response.ResponseCode = 201;
                    response.Result = "Pass";
                    response.Message = "Customer Updated Successfuly";  
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Result= "Fail";
                    response.Message = "Data Not Found";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Result = "Fail";
                response.Message = ex.Message;
            }
            return response;
        }

        
    }
}

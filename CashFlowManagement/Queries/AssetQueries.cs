﻿using CashFlowManagement.EntityModel;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class AssetQueries
    {
        public static AssetListViewModel GetAssetByUser(string username, int type)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            Entities entities = new Entities();
            IQueryable<AssetViewModel> queryResult = from asset in entities.Assets
                                                join income in entities.Incomes on asset.Id equals income.AssetId
                                                join bankdeposit in entities.BankDeposits on asset.Id equals bankdeposit.AssetId
                                                where asset.Username == username && income.IncomeType == type
                                                select new AssetViewModel { Asset = asset, Income = income, SpecificAsset = bankdeposit };
            AssetListViewModel result = new AssetListViewModel
            {
                Type = type,
                List = queryResult
            };
            return result;
        }

        public static int CreateAsset(AssetViewModel data, int type)
        {
            Entities entities = new Entities();
            int result = entities.SaveChanges();
            return result;
        }

        //public static int UpdateBankDeposit(BankDepositIncomes data)
        //{
        //    CashFlowManagementEntities entities = new CashFlowManagementEntities();
        //    BankDepositIncomes business = entities.BankDepositIncomes.Where(x => x.Id == data.Id).FirstOrDefault();
        //    DateTime current = DateTime.Now;

        //    business.EndDate = new DateTime(current.Year, current.Month, 1);
        //    entities.BankDepositIncomes.Attach(business);
        //    var entry = entities.Entry(business);
        //    entry.Property(x => x.EndDate).IsModified = true;

        //    BankDepositIncomes updated_bankdeposit = new BankDepositIncomes();
        //    updated_bankdeposit.Source = data.Source;
        //    updated_bankdeposit.CapitalValue = data.CapitalValue;
        //    updated_bankdeposit.InterestYield = data.InterestYield;
        //    updated_bankdeposit.ParticipantBank = data.ParticipantBank;
        //    updated_bankdeposit.Note = data.Note;
        //    updated_bankdeposit.StartDate = new DateTime(current.Year, current.Month, 1);
        //    updated_bankdeposit.Username = business.Username;
        //    entities.BankDepositIncomes.Add(updated_bankdeposit);

        //    int result = entities.SaveChanges();
        //    return result;
        //}

        //public static int DeleteBankDeposit(int id)
        //{
        //    CashFlowManagementEntities entities = new CashFlowManagementEntities();
        //    BankDepositIncomes deposit = entities.BankDepositIncomes.Where(x => x.Id == id).FirstOrDefault();
        //    DateTime current = DateTime.Now;
        //    deposit.EndDate = new DateTime(current.Year, current.Month, 1);
        //    entities.BankDepositIncomes.Attach(deposit);
        //    var entry = entities.Entry(deposit);
        //    entry.Property(x => x.EndDate).IsModified = true;
        //    int result = entities.SaveChanges();
        //    return result;
        //}

        //public static BankDepositIncomes GetBankDepositById(int id)
        //{
        //    CashFlowManagementEntities entities = new CashFlowManagementEntities();
        //    BankDepositIncomes deposit = entities.BankDepositIncomes.Where(x => x.Id == id).FirstOrDefault();
        //    return deposit;
        //}
    }
}
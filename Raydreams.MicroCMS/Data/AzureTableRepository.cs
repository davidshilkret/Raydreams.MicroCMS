﻿using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos.Table;

namespace Raydreams.MicroCMS
{
	/// <summary>Base class for accessing Azure Table Storage</summary>
	/// <typeparam name="T">The type stored in the table</typeparam>
	public class AzureTableRepository<T> where T : ITableEntity, new()
	{
		#region [Fields]

		/// <summary></summary>
		private string _connStr = String.Empty;

		/// <summary></summary>
		private string _table = String.Empty;

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		public AzureTableRepository( string connStr, string table )
		{
			this._connStr = connStr;
			this.TableName = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The physical Collection name</summary>
		public string TableName
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		/// <summary>The physical Collection name</summary>
		public CloudTable AzureTable
		{
			get
			{
				CloudStorageAccount account = CloudStorageAccount.Parse( this._connStr );
				CloudTableClient client = account.CreateCloudTableClient();
				CloudTable tbl = client.GetTableReference( this.TableName );
				return tbl;
			}
		}

		#endregion [Properties]

		/// <summary>Get all the items in the table with no filter at this time</summary>
		/// <remarks>This doesn't get everything unless you loop the token - see DeleteAll</remarks>
		/// <returns></returns>
		public List<T> List()
		{
			TableContinuationToken tok = new TableContinuationToken();
			TableQuerySegment<T> results = this.AzureTable.ExecuteQuerySegmentedAsync<T>( new TableQuery<T>(), tok ).GetAwaiter().GetResult();

			return results.Results;
		}

		/// <summary>Updates a single item in the table</summary>
		/// <returns>The HTTP Status Code response code.</returns>
		public int Update( T item )
		{
			if ( item == null )
				return 0;

			TableOperation op = TableOperation.Replace( item );
			TableResult results = this.AzureTable.ExecuteAsync( op ).GetAwaiter().GetResult();

			return results.HttpStatusCode;
		}

		/// <summary>Insert a single item</summary>
		/// <param name="item">The item to insert</param>
		/// <returns>The HTTP Status Code response code.</returns>
		public int Insert( T item )
		{
			if ( item == null )
				return (int)HttpStatusCode.BadRequest;

			TableResult results = null;

			try
			{
				TableOperation op = TableOperation.InsertOrMerge( item );
				results = this.AzureTable.ExecuteAsync( op ).GetAwaiter().GetResult();
			}
			catch ( System.Exception exp )
			{
				; // log it somehow and keep going
			}

			return results.HttpStatusCode;
		}

		/// <summary>Inserts a list of items into the table</summary>
		/// <param name="items">Items to insert in bulk</param>
		/// <returns>The HTTP Status Code response code.</returns>
		public int Insert( List<T> items )
		{
			if ( items == null || items.Count < 1 )
				return (int)HttpStatusCode.BadRequest;

			TableResult results = null;

			foreach ( T item in items )
			{
				try
				{
					TableOperation op = TableOperation.InsertOrMerge( item );
					results = this.AzureTable.ExecuteAsync( op ).GetAwaiter().GetResult();
				}
				catch ( System.Exception exp )
				{
					; // log it somehow and keep going
				}
			}

			return results.HttpStatusCode;
		}

		/// <summary>This will delete every record in a table and is very dangerous</summary>
		/// <param name="table"></param>
		/// <param name="client"></param>
		protected void DeleteAll()
		{
			TableContinuationToken token = null;
			CloudTable tbl = this.AzureTable;
			var query = new TableQuery<T>();
			TableResult results = null;

			do
			{
				var data = tbl.ExecuteQuerySegmentedAsync<T>( query, token ).GetAwaiter().GetResult();

				foreach ( var row in data )
				{
					var op = TableOperation.Delete( row );
					results = tbl.ExecuteAsync( op ).GetAwaiter().GetResult();
				}

				token = data.ContinuationToken;

			} while ( token != null );
		}
	}
}

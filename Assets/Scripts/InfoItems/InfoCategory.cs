using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.DataManagement;
using Assets.Graphics;
using Assets.Positional;
using UnityEngine;

namespace Assets.InfoItems
{
    abstract class InfoCategory
    {
        protected string name;
        protected Dictionary<string, InfoItem> infoItems = new Dictionary<string, InfoItem>();

        protected DataType dataType;
        protected DisplayArea displayArea;

        protected DTO lastDTO;

        public InfoCategory(
            string name,
            Player aligner,
            DataType dataType, DisplayArea displayArea)
        {
            this.name = name;
            this.dataType = dataType;
            this.displayArea = displayArea;
        }

        public string Name
        {
            get { return this.name; }
        }

        public DisplayArea DisplayArea
        {
            get { return this.displayArea; }
        }

        public DataType DataType
        {
            get { return this.dataType; }
        }

        public List<InfoItem> Update()
        {
            RetrieveInfoItems();
            Tick();
            return infoItems.Values.ToList();
        }

        protected void Tick()
        {
            foreach (InfoItem infoItem in infoItems.Values)
            {
                infoItem.Update();
            }
#if UNITY_EDITOR
            //this was added to keep the console clean.
            if (Player.Instance.debugVesselInfoInEditor)
                Debug.Log($"There are {infoItems.Count} InfoItems in the scene.");
#endif
        }

        protected abstract void RetrieveInfoItems();

        public abstract void OnDestroy();

        protected bool IsInInfoItems(InfoItem i)
        {
            return infoItems.ContainsKey(i.Key);
        }

        protected void AddNewInfoItem(InfoItem infoItem)
        {
            infoItems[infoItem.Key] = infoItem;
        }
    }

    class ConnectedInfoCategory : InfoCategory
    {
        private DataRetriever dataRetriever;

        public ConnectedInfoCategory(
            string name,
            Player aligner,
            DataType dataType, DisplayArea displayArea,
            DataConnections connection, DataAdapters adapter, ParameterExtractors extractor)
            : base(name, aligner, dataType, displayArea)
        {
            this.dataRetriever = new DataRetriever(connection, adapter, extractor, aligner);
        }

        protected override async void RetrieveInfoItems()
        {
            DTO dto = null;
            // If we can connect, get data
            if (dataRetriever.isConnected()) dto = await this.dataRetriever.fetch();

            // If we could connect and data is valid, store that data
            if (dto != null && dto.Valid)
            {
                lastDTO = dto;
                // Process new data is that was available, otherwise process old data if that is available
                HandleNewInfoItems(dto);
            }
        }

        public override void OnDestroy()
        {
            dataRetriever.OnDestroy();
        }
        private void HandleNewInfoItems(DTO dto)
        {
            //if (((AISDTOs)dto).vessels.Length > 0)
            //{
            //    Vector2 LatLon = Player.Instance.GetLatLon;
            //    ((AISDTOs)dto).vessels[0] = new AISDTO
            //    {
            //        Valid = true,
            //        SOG = 0,
            //        COG = 0,
            //        Draught = 0,
            //        Name = "NORTH",
            //        Key = "NORTH",
            //        Target = true,
            //        Latitude = LatLon.x + 1,
            //        Longitude = LatLon.y
            //    };
            //}

            foreach (InfoItem infoItem in AISInfoItem.Generate(dto, dataType, displayArea))
            {
                if (IsInInfoItems(infoItem))
                {
                    infoItems[infoItem.Key].InjectNewDTO(infoItem.GetDTO);
                    //infoItems[infoItem.Key].TargetNum = infoItem.TargetNum;
                }
                else
                {
                    AddNewInfoItem(infoItem);
                }
            }
            foreach (InfoItem infoItem in NAVAIDInfoItem.Generate(dto, dataType, displayArea))
            {
                if (IsInInfoItems(infoItem))
                {
                    infoItems[infoItem.Key].InjectNewDTO(infoItem.GetDTO);
                    //infoItems[infoItem.Key].TargetNum = infoItem.TargetNum;
                }
                else
                {
                    AddNewInfoItem(infoItem);
                }
            }
        }
    }

    class InjectedInfoCategory : InfoCategory
    {
        Func<List<InfoItem>> InfoItemInjector;

        public InjectedInfoCategory(
            string name,
            Player aligner,
            DataType dataType, DisplayArea displayArea,
            Func<List<InfoItem>> InfoItemInjector)
            : base(name, aligner, dataType, displayArea)
        {
            this.InfoItemInjector = InfoItemInjector;
        }

        protected override void RetrieveInfoItems()
        {
            List<InfoItem> newInfoItems = this.InfoItemInjector();

            foreach (InfoItem i in newInfoItems)
            {
                HandleNewInfoItem(i);
            }
        }

        public override void OnDestroy()
        {
            //dataRetriever.OnDestroy();
        }

        private void HandleNewInfoItem(InfoItem infoItem)
        {
            if (infoItem.DesiredState != ExpandState.Collapsed)
            {
                if (IsInInfoItems(infoItem))
                {
                    infoItems[infoItem.Key].InjectNewDTO(infoItem.GetDTO);
                    infoItems[infoItem.Key].TargetNum = infoItem.TargetNum;
                }
                else
                {
                    InfoItem newInfoItem = new AISInfoItem(
                        infoItem.GetDTO,
                        dataType,
                        displayArea);
                    //newInfoItem.IsTarget = infoItem.IsTarget;
                    newInfoItem.TargetNum = infoItem.TargetNum;
                    //newInfoItem.IsExpanded = infoItem.IsExpanded;
                    //newInfoItem.CurrentState = infoItem.DesiredState;
                    //newInfoItem.DesiredState = infoItem.DesiredState;
                    newInfoItem.Update();
                    newInfoItem.LinkTargetHandler(infoItem);
                    infoItem.LinkTargetHandler(newInfoItem);

                    AddNewInfoItem(newInfoItem);
                }
            }
            else
            {
                if (IsInInfoItems(infoItem))
                {
                    infoItems[infoItem.Key].DestroyMesh();
                    infoItems.Remove(infoItem.Key);
                }
            }
        }
    }
}

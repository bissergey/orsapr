﻿using System;
using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;

namespace KompasPlugin
{
    /// <summary>
    /// Класс осуществляющий построение детали
    /// </summary>
    public class WaveguideBuilder
    {
        /// <summary>
        /// Объект класса конектора для связи с КОММПАС-3D
        /// </summary>
        private KompasConnector _connector;

        /// <summary>
        /// Объект класса параметра для построение детали
        /// </summary>
        private WaveguideParameters _parameters;

        /// <summary>
        /// Стиль линии: основная
        /// </summary>
        private const int MainLineStyle = 1;

        /// <summary>
        /// Метод собирающий сечение и крепления в волновод
        /// </summary>
        public void BuildWaveguide()
        {
            //в центр всё перенести
            _connector.Start();
            _connector.CreateDocument3D();
            //Построение первого крепления
            BuildAnchorage(_parameters.AnchorageHeight,
                _parameters.AnchorageThickness,
                _parameters.AnchorageWidth,
                _parameters.DistanceAngleToHole,
                _parameters.HoleDiameters,
                Obj3dType.o3d_planeXOZ, null);

            //Скругление первого крепления
            var xCoordOfEdge = _parameters.AnchorageWidth / 2;
            var yCoordOfEdge = _parameters.AnchorageThickness / 2;
            var zCoordOfEdge = _parameters.AnchorageHeight / 2;

            CreateFillet(_parameters.RadiusCrossTie,
                xCoordOfEdge, yCoordOfEdge, zCoordOfEdge);
            CreateFillet(_parameters.RadiusCrossTie,
                -xCoordOfEdge, yCoordOfEdge, -zCoordOfEdge);
            CreateFillet(_parameters.RadiusCrossTie,
                -xCoordOfEdge, yCoordOfEdge, zCoordOfEdge);
            CreateFillet(_parameters.RadiusCrossTie,
                xCoordOfEdge, yCoordOfEdge, -zCoordOfEdge);

            //Построение сечения
            BuildCrossSection(_parameters.CrossSectionHeight,
                _parameters.CrossSectionThickness,
                _parameters.CrossSectionWidth,
                _parameters.WaveguideLength,
                Obj3dType.o3d_planeXOZ, null);

            //Смещени плоскости для построения второго крепления
            var offsetEntity = CreateOffsetPlane(
                Obj3dType.o3d_planeXOZ,
                -_parameters.WaveguideLength
                + _parameters.AnchorageThickness);

            //Создание второго крепления
            BuildAnchorage(_parameters.AnchorageHeight,
                _parameters.AnchorageThickness,
                _parameters.AnchorageWidth,
                _parameters.DistanceAngleToHole,
                _parameters.HoleDiameters,
                Obj3dType.o3d_planeXOZ, offsetEntity);

            //Скругление второго крепления
            xCoordOfEdge = _parameters.AnchorageWidth / 2;
            yCoordOfEdge = _parameters.WaveguideLength
                - _parameters.AnchorageThickness / 2;
            zCoordOfEdge = _parameters.AnchorageHeight / 2;

            CreateFillet(_parameters.RadiusCrossTie,
                xCoordOfEdge, yCoordOfEdge, zCoordOfEdge);
            CreateFillet(_parameters.RadiusCrossTie,
                -xCoordOfEdge, yCoordOfEdge, -zCoordOfEdge);
            CreateFillet(_parameters.RadiusCrossTie,
                -xCoordOfEdge, yCoordOfEdge, zCoordOfEdge);
            CreateFillet(_parameters.RadiusCrossTie,
                xCoordOfEdge, yCoordOfEdge, -zCoordOfEdge);
        }

        /// <summary>
        /// Метод построения крепления
        /// </summary>
        /// <param name="height">Высота</param>
        /// <param name="thickness">Толщина</param>
        /// <param name="width">Ширина</param>
        /// <param name="distanceAngleToHole">Расстояие от угла</param>
        /// <param name="holeDiameters">Диаметр отверстия</param>
        /// <param name="offsetPlane">Смещение плоскости</param>
        private void BuildAnchorage(double height, double thickness,
            double width, double distanceAngleToHole,
            double holeDiameters, Obj3dType planeType,
            ksEntity offsetPlane)
        {
            var sketch = CreateSketch(planeType, offsetPlane);
            var doc2d = (ksDocument2D)sketch.BeginEdit();

            //Создание внтуреннего контура
            var xCoordInternalRectangle =
                -(width - WaveguideParameters
                .ANCHORAGE_CROSS_SECTION_DIFFERENCE) / 2;
            var yCoordInternalRectangle =
                -(height - WaveguideParameters
                .ANCHORAGE_CROSS_SECTION_DIFFERENCE) / 2;

            doc2d.ksRectangle(DrawRectangle(xCoordInternalRectangle,
                yCoordInternalRectangle,
                height -
                WaveguideParameters.ANCHORAGE_CROSS_SECTION_DIFFERENCE,
                width -
                WaveguideParameters.ANCHORAGE_CROSS_SECTION_DIFFERENCE),
                0);

            //Создание внешнего контура
            doc2d.ksRectangle(DrawRectangle(-width / 2,
                -height / 2, height, width), 0);

            //Создание кругов для отвестий
            var cathet = Math.Sqrt(
                (distanceAngleToHole * distanceAngleToHole) / 2);

            var x = xCoordInternalRectangle - cathet;
            var y = yCoordInternalRectangle - cathet;

            doc2d.ksCircle(x, y, holeDiameters / 2, MainLineStyle);
            doc2d.ksCircle(-x, -y, holeDiameters / 2, MainLineStyle);
            doc2d.ksCircle(-x, y, holeDiameters / 2, MainLineStyle);
            doc2d.ksCircle(x, -y, holeDiameters / 2, MainLineStyle);

            //Выдавливание крепления
            sketch.EndEdit();
            СreateExtrusion(sketch, thickness);
        }

        /// <summary>
        /// Метод построения сечения
        /// </summary>
        /// <param name="height">Высота</param>
        /// <param name="thickness">Толщина стенок</param>
        /// <param name="width">Ширина</param>
        /// <param name="length">Длина</param>
        private void BuildCrossSection(double height, double thickness,
            double width, double length, Obj3dType planeType,
            ksEntity offsetPlane)
        {
            var sketch = CreateSketch(planeType, offsetPlane);
            var doc2d = (ksDocument2D)sketch.BeginEdit();

            //Создание внтуреннего контура
            doc2d.ksRectangle(DrawRectangle(-width / 2,
                -height / 2, height, width), 0);

            //Создание внешнего контура
            doc2d.ksRectangle(DrawRectangle(-width / 2 - thickness,
                -height / 2 - thickness, height + thickness * 2,
                width + thickness * 2), 0);

            //Выдавливание сечения
            sketch.EndEdit();
            СreateExtrusion(sketch, length);
        }

        /// <summary>
        /// Метод создающий эскиз
        /// </summary>
        /// <param name="planeType">Плоскость</param>
        /// <param name="offsetPlane">Объект смещения</param>
        /// <returns>Эскиз</returns>
        private ksSketchDefinition CreateSketch(Obj3dType planeType,
            ksEntity offsetPlane)
        {
            var plane = (ksEntity)_connector.Part
                .GetDefaultEntity((short)planeType);

            var sketch = (ksEntity)_connector.Part.
                NewEntity((short)Obj3dType.o3d_sketch);
            var ksSketch = (ksSketchDefinition)sketch.GetDefinition();

            if (offsetPlane != null)
            {
                ksSketch.SetPlane(offsetPlane);
                sketch.Create();
                return ksSketch;
            }

            ksSketch.SetPlane(plane);
            sketch.Create();
            return ksSketch;
        }

        /// <summary>
        /// Метод осущетсвляющий выдавливание
        /// </summary>
        /// <param name="sketch">Эскиз</param>
        /// <param name="depth">Расстояние выдавливания</param>
        private void СreateExtrusion(ksSketchDefinition sketch,
            double depth, bool side = true)
        {
            var extrusionEntity = (ksEntity)_connector.Part.NewEntity(
                (short)ksObj3dTypeEnum.o3d_bossExtrusion);
            var extrusionDef = (ksBossExtrusionDefinition)extrusionEntity
                .GetDefinition();

            extrusionDef.SetSideParam(side,
                (short)End_Type.etBlind, depth);
            extrusionDef.directionType = side ?
                (short)Direction_Type.dtNormal :
                (short)Direction_Type.dtReverse;
            extrusionDef.SetSketch(sketch);

            extrusionEntity.Create();
        }

        /// <summary>
        /// Метод осуществляющий вырезание
        /// </summary>
        /// <param name="sketch">Эскиз</param>
        /// <param name="depth">Расстояние выреза</param>
        private void СreateCutExtrusion(ksSketchDefinition sketch,
            double depth, bool side = true)
        {
            var cutExtrusionEntity = (ksEntity)_connector.Part.NewEntity(
                (short)ksObj3dTypeEnum.o3d_cutExtrusion);
            var cutExtrusionDef =
                (ksCutExtrusionDefinition)cutExtrusionEntity
                    .GetDefinition();

            cutExtrusionDef.SetSideParam(side,
                (short)End_Type.etBlind, depth);
            cutExtrusionDef.directionType = side ?
                (short)Direction_Type.dtNormal :
                (short)Direction_Type.dtReverse;
            cutExtrusionDef.cut = true;
            cutExtrusionDef.SetSketch(sketch);

            cutExtrusionEntity.Create();
        }

        /// <summary>
        /// Метод рисования прямоугольника
        /// </summary>
        /// <param name="x">X базовой точки</param>
        /// <param name="y">Y базовой точки</param>
        /// <param name="height">Высота</param>
        /// <param name="width">Ширина</param>
        /// <returns>Переменная с параметрами прямоугольника</returns>
        private ksRectangleParam DrawRectangle(double x, double y,
            double height, double width)
        {
            var rectangleParam =
                (ksRectangleParam)_connector.Object.GetParamStruct
                    ((short)StructType2DEnum.ko_RectangleParam);
            rectangleParam.x = x;
            rectangleParam.y = y;
            rectangleParam.height = height;
            rectangleParam.width = width;
            rectangleParam.style = MainLineStyle;
            return rectangleParam;
        }

        /// <summary>
        /// Создания фаски на выбранном ребре
        /// </summary>
        /// <param name="radiusCrossTie">Радиус</param>
        /// <param name="x">X-координата точки на ребре</param>
        /// <param name="y">Y-координата точки на ребре</param>
        /// <param name="z">Z-координата точки на ребре</param>
        private void CreateFillet(double radiusCrossTie, double x,
            double y, double z)
        {
            var filletEntity = (ksEntity)_connector
                .Part.NewEntity((short)Obj3dType.o3d_fillet);
            var filletDef =
                (ksFilletDefinition)filletEntity.GetDefinition();
            filletDef.radius = radiusCrossTie;
            filletDef.tangent = true;
            ksEntityCollection iArray = filletDef.array();
            ksEntityCollection iCollection = _connector
                .Part.EntityCollection((short)Obj3dType.o3d_edge);

            iCollection.SelectByPoint(x, y, z);
            var iEdge = iCollection.Last();
            iArray.Add(iEdge);
            filletEntity.Create();
        }

        /// <summary>
        /// Метод смещающий плоскость
        /// </summary>
        /// <param name="plane">Плоскость</param>
        /// <param name="offset">Расстояние смещения</param>
        /// <returns>Объект смещения</returns>
        private ksEntity CreateOffsetPlane(Obj3dType plane, double offset)
        {
            var offsetEntity = (ksEntity)_connector
                .Part.NewEntity((short)Obj3dType.o3d_planeOffset);
            var offsetDef = (ksPlaneOffsetDefinition)offsetEntity
                .GetDefinition();
            offsetDef.SetPlane((ksEntity)_connector
                .Part.NewEntity((short)plane));
            offsetDef.offset = offset;
            offsetDef.direction = false;
            offsetEntity.Create();
            return offsetEntity;
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="parameters">Параметры волновода</param>
        /// <param name="connector">Объект для связи с КОМПАС-3D</param>
        public WaveguideBuilder(WaveguideParameters parameters,
            KompasConnector connector)
        {
            _parameters = parameters;
            _connector = connector;
        }
    }
}

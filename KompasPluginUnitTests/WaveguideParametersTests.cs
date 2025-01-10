using System;
using System.Collections.Generic;
using NUnit.Framework;
using KompasPlugin;

namespace KompasPluginUnitTests
{
    //��������� �������
    [TestFixture]
    public class WaveguideParametersTests
    {
        /// <summary>
        /// ������ ������ � ����������� ��� ������
        /// </summary>
        private WaveguideParameters _testWaveguideParameters;

        /// <summary>
        /// ���������� ������� "�� ���������� �� �������� ����������"
        /// </summary>
        private bool IsParametersNotDefault(
            WaveguideParameters testWaveguideParameters)
        {
            return (_testWaveguideParameters.AnchorageHeight
                != WaveguideParameters.MIN_ANCHORAGE_HEIGHT
                || _testWaveguideParameters.AnchorageWidth
                != WaveguideParameters.MIN_ANCHORAGE_WIDTH
                || _testWaveguideParameters.CrossSectionHeight
                != WaveguideParameters.MIN_CROSS_SECTION_HEIGHT
                || _testWaveguideParameters.CrossSectionWidth
                != WaveguideParameters.MIN_CROSS_SECTION_WIDTH);
        }

        /// <summary>
        /// ���������� ������� "���������� �� �������� ����������"
        /// </summary>
        private bool IsParametersChanged(
            WaveguideParameters testWaveguideParameters)
        {
            return (testWaveguideParameters.AnchorageHeight
                == WaveguideParameters.MAX_ANCHORAGE_HEIGHT
                && testWaveguideParameters.AnchorageWidth
                == WaveguideParameters.MAX_ANCHORAGE_WIDTH
                && testWaveguideParameters.CrossSectionHeight
                == WaveguideParameters.MAX_CROSS_SECTION_HEIGHT
                && testWaveguideParameters.CrossSectionWidth
                == WaveguideParameters.MAX_CROSS_SECTION_WIDTH);
        }

        /// <summary>
        /// ������� ��� � ������������ �������� ����������
        /// </summary>
        private Dictionary<ParameterNames, double>
            _maxValuesOfParameterDictionary =
                new Dictionary<ParameterNames, double>()
                {
                    {
                        ParameterNames.AnchorageHeight,
                        WaveguideParameters.MAX_ANCHORAGE_HEIGHT
                    },
                    {
                        ParameterNames.AnchorageThickness,
                        WaveguideParameters.MAX_ANCHORAGE_THICKNESS
                    },
                    {
                        ParameterNames.AnchorageWidth,
                        WaveguideParameters.MAX_ANCHORAGE_WIDTH
                    },
                    {
                        ParameterNames.CrossSectionHeight,
                        WaveguideParameters.MAX_CROSS_SECTION_HEIGHT
                    },
                    {
                        ParameterNames.CrossSectionThickness,
                        WaveguideParameters.MAX_CROSS_SECTION_THICKNESS
                    },
                    {
                        ParameterNames.CrossSectionWidth,
                        WaveguideParameters.MAX_CROSS_SECTION_WIDTH
                    },
                    {
                        ParameterNames.DistanceAngleToHole,
                        WaveguideParameters.MAX_DISTANCE_ANGLE_TO_HOLE
                    },
                    {
                        ParameterNames.HoleDiameters,
                        WaveguideParameters.MAX_HOLE_DIAMETERS
                    },
                    {
                        ParameterNames.RadiusCrossTie,
                        WaveguideParameters.MAX_RADIUS_CROSS_TIE
                    },
                    {
                        ParameterNames.WaveguideLenght,
                        WaveguideParameters.MAX_WAVEGUIDE_LENGTH
                    },
                };

        /// <summary>
        /// ��������� ��� ������� ������������ ��������
        /// </summary>
        private static string _uncorrectSetterErrorMessage =
                "���������, ���� �������� ����������, ����� �� ������ " +
                "���� ��� �������� ����������, ����� ������ ���� " +
                "����������";

        [Test(Description = "���� �� ������ ������ ���������")]
        public void TestAnchorageHeightSet_SetMaxAndDefault()
        {
            _testWaveguideParameters = new WaveguideParameters();
            _testWaveguideParameters.AnchorageHeight =
                WaveguideParameters.MIN_ANCHORAGE_HEIGHT;
            var negativeTestResult =
                !IsParametersNotDefault(_testWaveguideParameters);
            _testWaveguideParameters.AnchorageHeight =
                WaveguideParameters.MAX_ANCHORAGE_HEIGHT;
            var positiveTestResult =
                IsParametersChanged(_testWaveguideParameters);

            Assert.IsTrue(negativeTestResult && positiveTestResult,
                _uncorrectSetterErrorMessage);
        }

        [Test(Description = "���� �� ������ ������ ���������")]
        public void TestAnchorageWidthtSet_SetMaxAndDefault()
        {
            _testWaveguideParameters = new WaveguideParameters();
            _testWaveguideParameters.AnchorageWidth =
                WaveguideParameters.MIN_ANCHORAGE_WIDTH;
            var negativeTestResult =
                !IsParametersNotDefault(_testWaveguideParameters);
            _testWaveguideParameters.AnchorageWidth =
                WaveguideParameters.MAX_ANCHORAGE_WIDTH;
            var positiveTestResult =
                IsParametersChanged(_testWaveguideParameters);

            Assert.IsTrue(negativeTestResult && positiveTestResult,
                _uncorrectSetterErrorMessage);
        }

        [Test(Description = "���� �� ������ ������ �������")]
        public void TestCrossSectionHeightSet_SetMaxAndDefault()
        {
            _testWaveguideParameters = new WaveguideParameters();
            _testWaveguideParameters.CrossSectionHeight =
                WaveguideParameters.MIN_CROSS_SECTION_HEIGHT;
            var negativeTestResult =
                !IsParametersNotDefault(_testWaveguideParameters);
            _testWaveguideParameters.CrossSectionHeight =
                WaveguideParameters.MAX_CROSS_SECTION_HEIGHT;
            var positiveTestResult =
                IsParametersChanged(_testWaveguideParameters);

            Assert.IsTrue(negativeTestResult && positiveTestResult,
                _uncorrectSetterErrorMessage);
        }

        [Test(Description = "���� �� ������ ������ �������")]
        public void TestCrossSectionWidthSet_SetMaxAndDefault()
        {
            _testWaveguideParameters = new WaveguideParameters();
            _testWaveguideParameters.CrossSectionWidth =
                WaveguideParameters.MIN_CROSS_SECTION_WIDTH;
            var negativeTestResult =
                !IsParametersNotDefault(_testWaveguideParameters);
            _testWaveguideParameters.CrossSectionWidth =
                WaveguideParameters.MAX_CROSS_SECTION_WIDTH;
            var positiveTestResult =
                IsParametersChanged(_testWaveguideParameters);

            Assert.IsTrue(negativeTestResult && positiveTestResult,
                _uncorrectSetterErrorMessage);
        }

        [Test(Description = "���� ������ ���������� �������� "
            + "� ������ ��������� �� ��� �����")]
        public void TestSetParameterByName()
        {
            _testWaveguideParameters = new WaveguideParameters();

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                _testWaveguideParameters.SetParameterByName(
                    parameterMaxValue.Key, parameterMaxValue.Value);
            }

            int errorCounter = 0;

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                if (_testWaveguideParameters.GetParameterValueByName(
                          parameterMaxValue.Key) != parameterMaxValue.Value)
                {
                    errorCounter++;
                }
            }

            Assert.Zero(errorCounter,
                "�������� �� ���� �������� � ������� ����������");
        }

        [Test(Description = "���� �� ������ �������� ��������� �� �����")]
        public void TestGetParameterByName()
        {
            _testWaveguideParameters = new WaveguideParameters();

            var newValue = (WaveguideParameters.MIN_HOLE_DIAMETERS
                            + WaveguideParameters.MAX_HOLE_DIAMETERS) / 2;
            ParameterNames testParameterName =
                ParameterNames.HoleDiameters;
            _testWaveguideParameters
                .SetParameterByName(testParameterName, newValue);

            Assert.AreEqual(newValue, _testWaveguideParameters
                    .GetParameterValueByName(testParameterName),
                "�� ������� ��������� �������� ��������");
        }

        [Test(Description = "���������� ���� �� ������� ����������")]
        public void TestParameterGet()
        {
            _testWaveguideParameters = new WaveguideParameters();

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                _testWaveguideParameters.SetParameterByName(
                    parameterMaxValue.Key, parameterMaxValue.Value);
            }

            Assert.IsTrue(_testWaveguideParameters.AnchorageThickness
                == WaveguideParameters.MAX_ANCHORAGE_THICKNESS
                && _testWaveguideParameters.CrossSectionThickness
                == WaveguideParameters.MAX_CROSS_SECTION_THICKNESS
                && _testWaveguideParameters.HoleDiameters
                == WaveguideParameters.MAX_HOLE_DIAMETERS
                && _testWaveguideParameters.DistanceAngleToHole
                == WaveguideParameters.MAX_DISTANCE_ANGLE_TO_HOLE
                && _testWaveguideParameters.RadiusCrossTie
                == WaveguideParameters.MAX_RADIUS_CROSS_TIE
                && _testWaveguideParameters.WaveguideLength
                == WaveguideParameters.MAX_WAVEGUIDE_LENGTH,
                "���������, ���� ������ ������ �� �� ��������");
        }
    }
}

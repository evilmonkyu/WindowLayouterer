using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using WindowLayouterer.Configuration;
using WindowLayouterer.Domain;

namespace WindowLayouterer.Tests.Configuraton
{
    [TestClass]
    public class SettingsParserTests
    {
        private SettingsParser SettingsParser;

        [TestInitialize]
        public void Setup()
        {
            SettingsParser = new SettingsParser();
        }

        [TestMethod]
        public void Can_ParseSettings()
        {
            var result = SettingsParser.Parse(ExampleSettingsText);
            Compare(s => s.Layouts.Count, result);
            for (int i = 0; i < result.Layouts.Count; i++)
            {
                Compare(s => s.Layouts[i].Name, result);
                Compare(s => s.Layouts[i].HotKey, result);
                Compare(s => s.Layouts[i].IsDefault, result);
                Compare(s => s.Layouts[i].ScreenAreas.Count, result);
                for (int j = 0; j < result.Layouts[i].ScreenAreas.Count; j++)
                {
                    Compare(s => s.Layouts[i].ScreenAreas[j].Name, result);
                    Compare(s => s.Layouts[i].ScreenAreas[j].HotKey, result);
                    Compare(s => s.Layouts[i].ScreenAreas[j].Left, result);
                    Compare(s => s.Layouts[i].ScreenAreas[j].Top, result);
                    Compare(s => s.Layouts[i].ScreenAreas[j].Width, result);
                    Compare(s => s.Layouts[i].ScreenAreas[j].Height, result);
                    Compare(s => s.Layouts[i].ScreenAreas[j].Windows.Count, result);
                    for (int k = 0; k < result.Layouts[i].ScreenAreas[j].Windows.Count; k++)
                    {
                        Compare(s => s.Layouts[i].ScreenAreas[j].Windows[k].Name, result);
                        Compare(s => s.Layouts[i].ScreenAreas[j].Windows[k].Process, result);
                        Compare(s => s.Layouts[i].ScreenAreas[j].Windows[k].Title, result);
                    }
                }
            }
        }

        private void Compare(Func<Settings, object> comparison, Settings result)
        {
            Assert.AreEqual(comparison(ExampleSettings), comparison(result));
        }

        private string ExampleSettingsText = @"
{
    ""Windows"": [
        {
            ""Name"": ""notepad"",
            ""Process"": ""notepad.exe"",
            ""Title"": "".*\\.txt""
        },
        {
            ""Name"": ""chrome"",
            ""Process"": ""chrome.exe"",
        }
    ],
    ""ScreenAreas"": [
        {
            ""Name"": ""area1"",
            ""HotKey"": ""Ctrl+Alt+8"",
            ""Windows"": [
                ""notepad"",
                ""chrome""
            ],
            ""Left"": ""0"",
            ""Top"": ""10%"",
            ""Width"": ""30%"",
            ""Height"": ""50%""
        },
        {
            ""Name"": ""area2"",
            ""HotKey"": ""Ctrl+Alt+9"",
            ""Windows"": [
                ""notepad"",
                ""chrome""
            ],
            ""Left"": ""5%"",
            ""Top"": ""80%"",
            ""Width"": ""40.7%"",
            ""Height"": ""20.9%""
        }
    ],
    ""Layouts"": [
        {
            ""Name"": ""layout1"",
            ""HotKey"": ""Ctrl+]"",
            ""IsDefault"": ""true"",
            ""ScreenAreas"": [
                ""area1"",
                {
                    ""Name"": ""area2"",
                    ""HotKey"": ""Ctrl+Alt+6"",
                    ""Height"": ""40%""
                },
                {
                    ""Name"": ""area3"",
                    ""HotKey"": ""Ctrl+Alt+7"",
                    ""Windows"": [
                        ""notepad"",
                    ],
                    ""Left"": ""50%"",
                    ""Top"": ""50%"",
                    ""Width"": ""10%"",
                    ""Height"": ""15%""
                }
            ]
        },
        {
            ""Name"": ""layout2"",
            ""HotKey"": ""Ctrl+["",
            ""IsDefault"": ""false"",
            ""ScreenAreas"": [
                ""area1"",
                ""area2""
            ]
        }
    ]
}";
        private Settings ExampleSettings = new Settings
        {
            Layouts = new List<LayoutSetting>
            {
                new LayoutSetting
                {
                    Name = "layout1",
                    HotKey = new HotKey(),
                    IsDefault = true,
                    ScreenAreas = new List<ScreenAreaSetting>
                    {
                        new ScreenAreaSetting
                        {
                            Name = "area1",
                            HotKey = new HotKey(),
                            Windows = new List<WindowSetting>
                            {
                                new WindowSetting
                                {
                                    Name = "notepad",
                                    Process = "notepad.exe",
                                    Title = ".*\\.txt"
                                },
                                new WindowSetting
                                {
                                    Name = "chrome",
                                    Process = "chrome.exe",
                                }
                            },
                            Left = 0,
                            Top = 10,
                            Width = 30,
                            Height = 50
                        },
                        new ScreenAreaSetting
                        {
                            Name = "area2",
                            HotKey =  new HotKey(),
                            Windows = new List<WindowSetting>
                            {
                                new WindowSetting
                                {
                                    Name = "notepad",
                                    Process = "notepad.exe",
                                    Title = ".*\\.txt"
                                },
                                new WindowSetting
                                {
                                    Name = "chrome",
                                    Process = "chrome.exe",
                                }
                            },
                            Left = 5,
                            Top = 80,
                            Width = 40.7m,
                            Height = 40
                        },
                        new ScreenAreaSetting
                        {
                            Name = "area3",
                            HotKey =  new HotKey(),
                            Windows = new List<WindowSetting>
                            {
                                new WindowSetting
                                {
                                    Name = "notepad",
                                    Process = "notepad.exe",
                                    Title = ".*\\.txt"
                                }
                            },
                            Left = 50,
                            Top = 50,
                            Width = 10,
                            Height = 15
                        }
                    },
                },
                new LayoutSetting
                {
                    Name = "layout2",
                    HotKey = new HotKey(),
                    IsDefault = false,
                    ScreenAreas = new List<ScreenAreaSetting>
                    {
                        new ScreenAreaSetting
                        {
                            Name = "area1",
                            HotKey = new HotKey(),
                            Windows = new List<WindowSetting>
                            {
                                new WindowSetting
                                {
                                    Name = "notepad",
                                    Process = "notepad.exe",
                                    Title = ".*\\.txt"
                                },
                                new WindowSetting
                                {
                                    Name = "chrome",
                                    Process = "chrome.exe",
                                }
                            },
                            Left = 0,
                            Top = 10,
                            Width = 30,
                            Height = 50
                        },
                        new ScreenAreaSetting
                        {
                            Name = "area2",
                            HotKey =  new HotKey(),
                            Windows = new List<WindowSetting>
                            {
                                new WindowSetting
                                {
                                    Name = "notepad",
                                    Process = "notepad.exe",
                                    Title = ".*\\.txt"
                                },
                                new WindowSetting
                                {
                                    Name = "chrome",
                                    Process = "chrome.exe",
                                }
                            },
                            Left = 5,
                            Top = 80,
                            Width = 40.7m,
                            Height = 20.9m
                        },
                    }
                }
            }
        };
    }
}

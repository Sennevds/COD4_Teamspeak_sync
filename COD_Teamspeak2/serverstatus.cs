﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class B3Status {
    
    private B3StatusGame[] gameField;
    
    private B3StatusClients[] clientsField;
    
    private string timeField;
    
    private string timeStampField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Game", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public B3StatusGame[] Game {
        get {
            return this.gameField;
        }
        set {
            this.gameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Clients", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public B3StatusClients[] Clients {
        get {
            return this.clientsField;
        }
        set {
            this.clientsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Time {
        get {
            return this.timeField;
        }
        set {
            this.timeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TimeStamp {
        get {
            return this.timeStampField;
        }
        set {
            this.timeStampField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class B3StatusGame {
    
    private B3StatusGameData[] dataField;
    
    private string capatureLimitField;
    
    private string fragLimitField;
    
    private string mapField;
    
    private string mapTimeField;
    
    private string nameField;
    
    private string roundTimeField;
    
    private string roundsField;
    
    private string timeLimitField;
    
    private string typeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Data", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public B3StatusGameData[] Data {
        get {
            return this.dataField;
        }
        set {
            this.dataField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string CapatureLimit {
        get {
            return this.capatureLimitField;
        }
        set {
            this.capatureLimitField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string FragLimit {
        get {
            return this.fragLimitField;
        }
        set {
            this.fragLimitField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Map {
        get {
            return this.mapField;
        }
        set {
            this.mapField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string MapTime {
        get {
            return this.mapTimeField;
        }
        set {
            this.mapTimeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RoundTime {
        get {
            return this.roundTimeField;
        }
        set {
            this.roundTimeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Rounds {
        get {
            return this.roundsField;
        }
        set {
            this.roundsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TimeLimit {
        get {
            return this.timeLimitField;
        }
        set {
            this.timeLimitField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class B3StatusGameData {
    
    private string nameField;
    
    private string valueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class B3StatusClients {
    
    private B3StatusClientsClient[] clientField;
    
    private string totalField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Client", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public B3StatusClientsClient[] Client {
        get {
            return this.clientField;
        }
        set {
            this.clientField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Total {
        get {
            return this.totalField;
        }
        set {
            this.totalField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class B3StatusClientsClient {
    
    private string cIDField;
    
    private string colorNameField;
    
    private string dBIDField;
    
    private string ipField;
    
    private string idField;
    
    private string scoreField;
    
    private string killsField;
    
    private string deathsField;
    
    private string assistsField;
    
    private string pingField;
    
    private string teamField;
    
    private string teamNameField;
    
    private string updatedField;
    
    private string powerField;
    
    private string rankField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string CID {
        get {
            return this.cIDField;
        }
        set {
            this.cIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ColorName {
        get {
            return this.colorNameField;
        }
        set {
            this.colorNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string DBID {
        get {
            return this.dBIDField;
        }
        set {
            this.dBIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string IP {
        get {
            return this.ipField;
        }
        set {
            this.ipField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ID {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Score {
        get {
            return this.scoreField;
        }
        set {
            this.scoreField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Kills {
        get {
            return this.killsField;
        }
        set {
            this.killsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Deaths {
        get {
            return this.deathsField;
        }
        set {
            this.deathsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Assists {
        get {
            return this.assistsField;
        }
        set {
            this.assistsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Ping {
        get {
            return this.pingField;
        }
        set {
            this.pingField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Team {
        get {
            return this.teamField;
        }
        set {
            this.teamField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TeamName {
        get {
            return this.teamNameField;
        }
        set {
            this.teamNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Updated {
        get {
            return this.updatedField;
        }
        set {
            this.updatedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string power {
        get {
            return this.powerField;
        }
        set {
            this.powerField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string rank {
        get {
            return this.rankField;
        }
        set {
            this.rankField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class NewDataSet {
    
    private B3Status[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("B3Status")]
    public B3Status[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

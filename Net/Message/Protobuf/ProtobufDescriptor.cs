using System;
using System.Collections.Generic;
using Google.Protobuf;
public class ProtobufDescriptor {
	public delegate IMessage ParserDelegate(byte[] buffer);
	public static IMessage ParserFrom(short id, byte[] buffer) {
		ParserDelegate d = null;
		if(m_parsers.TryGetValue(id, out d)) {
			return d.Invoke(buffer);
		}
		return null;
	}
	private static Dictionary<short, ParserDelegate> m_parsers = new Dictionary<short, ParserDelegate>{
//		{NetMessageConst.UserLoginRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_UserLoginMsg},
//		{NetMessageConst.UserLoginRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_LobbyDataMsg},
//		{NetMessageConst.CreateRoleRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_CreateRoleMsg},
//		{NetMessageConst.LoginFailureRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_NotDataMsg},
//		{NetMessageConst.OtherLoginRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_NotDataMsg},
//		{NetMessageConst.CancelMatchRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_NotDataMsg},
//		{NetMessageConst.CancelMatchRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_NotDataMsg},
//		{NetMessageConst.ReplaceHeroRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_ReplaceHeroMsg},
//		{NetMessageConst.ReplaceHeroRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_NotDataMsg},
//		{NetMessageConst.StartGameRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_StartGameToClientMsg},
//		{NetMessageConst.GameOverRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_ClientGameOverMsg},
//		{NetMessageConst.LoadOverRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_LoadOverMsg},
//		{NetMessageConst.SelectPosRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_SelectPositionMsg},
//		{NetMessageConst.SelectPosRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_SelectPositionMsg},
//		{NetMessageConst.TeamGameOverRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_TeamGameOverMsg},
//		{NetMessageConst.NameExistRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_NotDataMsg},
//		{NetMessageConst.ErrorCodeRespone,Parser_ErrorCodeMsg},
//		{NetMessageConst.BeginMatchRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_StartMatchMsg},
//		{NetMessageConst.BeginMatchRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_ClientMatchMsg},
//		{NetMessageConst.MatchCountRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_ClientMatchMsg},
//		{NetMessageConst.PlayAgainRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_PlayAgainMsg},
//		{NetMessageConst.PlayAgainRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_PlayAgainMsg},
//		{NetMessageConst.ReturnMainResopne,Parser_Com_Dcsz_Ssjf_Grpc_Proto_NotDataMsg},
//		{NetMessageConst.HeartBeatRequest,Parser_Com_Dcsz_Ssjf_Grpc_Proto_HeartBeatMsg},
//		{NetMessageConst.HeartBeatRespone,Parser_Com_Dcsz_Ssjf_Grpc_Proto_HeartBeatMsg},
	};

//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_UserLoginMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.UserLoginMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_LobbyDataMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.LobbyDataMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_CreateRoleMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.CreateRoleMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_NotDataMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.NotDataMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_ReplaceHeroMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.ReplaceHeroMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_StartGameToClientMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.StartGameToClientMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_ClientGameOverMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.ClientGameOverMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_LoadOverMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.LoadOverMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_SelectPositionMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.SelectPositionMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_TeamGameOverMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.TeamGameOverMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_ErrorCodeMsg(byte[] buffer) { return ErrorCodeMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_StartMatchMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.StartMatchMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_ClientMatchMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.ClientMatchMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_PlayAgainMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.PlayAgainMsg.Descriptor.Parser.ParseFrom(buffer);}
//	private static IMessage Parser_Com_Dcsz_Ssjf_Grpc_Proto_HeartBeatMsg(byte[] buffer) { return Com.Dcsz.Ssjf.Grpc.Proto.HeartBeatMsg.Descriptor.Parser.ParseFrom(buffer);}
}

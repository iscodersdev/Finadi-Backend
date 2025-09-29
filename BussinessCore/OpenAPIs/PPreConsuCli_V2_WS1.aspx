<?xml version = "1.0" encoding = "utf-8"?>
<definitions name="PPreConsuCli_V2_WS" targetNamespace="GX" xmlns:wsdlns="GX" xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="http://schemas.xmlsoap.org/wsdl/" xmlns:tns="GX">
	<types>
		<schema targetNamespace="GX" xmlns="http://www.w3.org/2001/XMLSchema" xmlns:SOAP-ENC="http://schemas.xmlsoap.org/soap/encoding/" elementFormDefault="qualified">
			<complexType name="WSPreConsulta">
				<sequence>
					<element minOccurs="0" maxOccurs="unbounded" name="WSPreCons" type="tns:WSPreConsulta.WSPreCons">
					</element>
				</sequence>
			</complexType>
			<complexType name="WSPreConsulta.WSPreCons">
				<sequence>
					<element name="CUIT" type="xsd:string">
					</element>
					<element name="NroDePrestamo" type="xsd:int">
					</element>
					<element name="Secuencia" type="xsd:int">
					</element>
					<element name="CapitalFirmado" type="xsd:double">
					</element>
					<element name="CapitalNeto" type="xsd:double">
					</element>
					<element name="FechaLiquidacion" type="xsd:date" nillable="true">
					</element>
					<element name="CuotasTotales" type="xsd:short">
					</element>
					<element name="CuotasPagas" type="xsd:short">
					</element>
					<element name="CuotasRestantes" type="xsd:short">
					</element>
					<element name="CuotasEnMora" type="xsd:short">
					</element>
					<element name="ImporteEnMora" type="xsd:double">
					</element>
					<element name="FechaDeUltimoPago" type="xsd:date" nillable="true">
					</element>
					<element name="UltimaCuotaPaga" type="xsd:date" nillable="true">
					</element>
					<element name="SaldoParaCancelar" type="xsd:double">
					</element>
					<element name="SaldoParaRefinanciar" type="xsd:double">
					</element>
					<element name="PreSugeSalCan" type="xsd:double">
					</element>
					<element name="PreSugeSalCanPun" type="xsd:double">
					</element>
					<element name="PreSugeSalCanTot" type="xsd:double">
					</element>
					<element name="PreSugeSalRen" type="xsd:double">
					</element>
					<element name="PreSugeSalRenPun" type="xsd:double">
					</element>
					<element name="PreSugeSalRenTot" type="xsd:double">
					</element>
					<element name="CliDes" type="xsd:string">
					</element>
					<element name="TerrID" type="xsd:string">
					</element>
					<element name="LoLtID" type="xsd:string">
					</element>
					<element name="CtoCrDes" type="xsd:string">
					</element>
					<element name="PreEstDes" type="xsd:string">
					</element>
					<element name="PreEstDesSh" type="xsd:string">
					</element>
					<element name="PreEstLiqDes" type="xsd:string">
					</element>
					<element name="PreEstLiqDesSh" type="xsd:string">
					</element>
					<element name="PreVend2Des" type="xsd:string">
					</element>
					<element name="RiePedID" type="xsd:int">
					</element>
					<element name="PreRetRenov" type="xsd:double">
					</element>
					<element name="PreRetCtaSoc" type="xsd:double">
					</element>
					<element name="PreSisTasa" type="xsd:double">
					</element>
					<element name="preTEM" type="xsd:double">
					</element>
					<element name="PreCFT_V1_CapFirm" type="xsd:double">
					</element>
					<element name="PreCFT_V1_CapLiqui" type="xsd:double">
					</element>
					<element name="PreCFT_V1_CapNeto" type="xsd:double">
					</element>
					<element name="PrePriCta" type="xsd:double">
					</element>
					<element name="PreEstEsApr" type="xsd:string">
					</element>
					<element name="PreEstLiqEsLiqui" type="xsd:string">
					</element>
					<element name="ServiciosVigentes" type="xsd:double">
					</element>
					<element name="WSPreConsultaCuotas">
						<complexType>
							<sequence>
								<element minOccurs="0" maxOccurs="unbounded" name="WSPreConsCta" type="tns:WSPreConsulta.WSPreCons.WSPreConsCta">
								</element>
							</sequence>
						</complexType>
					</element>
				</sequence>
			</complexType>
			<complexType name="ArrayOfWSPreConsulta.WSPreCons.WSPreConsCta">
				<sequence>
					<element minOccurs="0" maxOccurs="unbounded" name="WSPreConsulta.WSPreCons.WSPreConsCta" type="tns:WSPreConsulta.WSPreCons.WSPreConsCta">
					</element>
				</sequence>
			</complexType>
			<complexType name="WSPreConsulta.WSPreCons.WSPreConsCta">
				<sequence>
					<element name="RenCtaNro" type="xsd:short">
					</element>
					<element name="RenCtaFeIni" type="xsd:date" nillable="true">
					</element>
					<element name="RenCtaFeVen" type="xsd:date" nillable="true">
					</element>
					<element name="RenCtaFePag" type="xsd:date" nillable="true">
					</element>
					<element name="RenCtaCap" type="xsd:double">
					</element>
					<element name="RenCtaInt" type="xsd:double">
					</element>
					<element name="RenCtaPura" type="xsd:double">
					</element>
					<element name="RenCtaSeguro" type="xsd:double">
					</element>
					<element name="RenCtaGas" type="xsd:double">
					</element>
					<element name="RenCtaIVA" type="xsd:double">
					</element>
					<element name="RenCtaCS" type="xsd:double">
					</element>
					<element name="RenCtaACC" type="xsd:double">
					</element>
					<element name="RenCtaOtros" type="xsd:double">
					</element>
					<element name="RenCtaTotal" type="xsd:double">
					</element>
					<element name="RenCtaQuita" type="xsd:double">
					</element>
					<element name="RenCtaRecargo" type="xsd:double">
					</element>
					<element name="RenCtaSaldoSinRec" type="xsd:double">
					</element>
					<element name="RenCtaSaldoFinal" type="xsd:double">
					</element>
					<element name="RenCtaMoraDias" type="xsd:int">
					</element>
					<element name="RenCtaMoraDiasAcu" type="xsd:int">
					</element>
					<element name="PreRCtaLiqCod" type="xsd:string">
					</element>
					<element name="PreCtaECod" type="xsd:string">
					</element>
					<element name="RenSalCap" type="xsd:double">
					</element>
					<element name="RenSalInt" type="xsd:double">
					</element>
					<element name="RenSalIntIVA" type="xsd:double">
					</element>
					<element name="RenSalGas" type="xsd:double">
					</element>
					<element name="RenSalGasIVA" type="xsd:double">
					</element>
					<element name="RenSalSeg" type="xsd:double">
					</element>
					<element name="RenPagosCta" type="xsd:double">
					</element>
					<element name="RenPagosPunt" type="xsd:double">
					</element>
					<element name="RenPagosTot" type="xsd:double">
					</element>
				</sequence>
			</complexType>
			<element name="PPreConsuCli_V2_WS.Execute">
				<complexType>
					<sequence>
						<element minOccurs="1" maxOccurs="1" name="Cuit" type="xsd:string" />
					</sequence>
				</complexType>
			</element>
			<element name="PPreConsuCli_V2_WS.ExecuteResponse">
				<complexType>
					<sequence>
						<element minOccurs="1" maxOccurs="1" name="Wspreconsulta" type="tns:WSPreConsulta" />
					</sequence>
				</complexType>
			</element>
		</schema>
	</types>
	<message name="PPreConsuCli_V2_WS.ExecuteSoapIn">
		<part name="parameters" element="tns:PPreConsuCli_V2_WS.Execute" />
	</message>
	<message name="PPreConsuCli_V2_WS.ExecuteSoapOut">
		<part name="parameters" element="tns:PPreConsuCli_V2_WS.ExecuteResponse" />
	</message>
	<portType name="PPreConsuCli_V2_WSSoapPort">
		<operation name="Execute">
			<input message="wsdlns:PPreConsuCli_V2_WS.ExecuteSoapIn" />
			<output message="wsdlns:PPreConsuCli_V2_WS.ExecuteSoapOut" />
		</operation>
	</portType>
	<binding name="PPreConsuCli_V2_WSSoapBinding" type="wsdlns:PPreConsuCli_V2_WSSoapPort">
		<soap:binding style="document" transport="http://schemas.xmlsoap.org/soap/http" />
		<operation name="Execute">
			<soap:operation soapAction="GXaction/APPRECONSUCLI_V2_WS.Execute" />
			<input>
				<soap:body use="literal" />
			</input>
			<output>
				<soap:body use="literal" />
			</output>
		</operation>
	</binding>
	<service name="PPreConsuCli_V2_WS">
		<port name="PPreConsuCli_V2_WSSoapPort" binding="wsdlns:PPreConsuCli_V2_WSSoapBinding">
			<soap:address location="http://190.210.231.192/ppreconsucli_v2_ws.aspx" />
		</port>
	</service>
</definitions>

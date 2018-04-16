#include "stdafx.h"
#include "crema_tables.h"

namespace cremacode
{
    Table_inerrantRow::Table_inerrantRow(CremaReader::irow& row, Table_inerrantTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Elicia = row.to_int8(0);
        if (row.has_value(1))
        {
            this->execrably = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->exclusionary = ((Type_rennet)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->bulky = ((Type15)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->scintillation = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->triffid = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->groveling = row.to_uint16(6);
        }
        this->SetKey(this->Elicia);
    }
    
    Table_inerrantTable::Table_inerrantTable()
    {
    }
    
    Table_inerrantTable::Table_inerrantTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_inerrantTable::~Table_inerrantTable()
    {
    }
    
    void* Table_inerrantTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_inerrantRow(row, ((Table_inerrantTable*)(table)));
    }
    
    const Table_inerrantRow* Table_inerrantTable::Find(char Elicia) const
    {
        return this->FindRow(Elicia);
    }
    
    Table204Row::Table204Row(CremaReader::irow& row, Table204Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->laundrymen = row.to_int16(0);
        if (row.has_value(1))
        {
            this->Donaugh = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->Bodhidharma = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->ample = row.to_int8(3);
        }
        if (row.has_value(4))
        {
            this->befoul = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->Taliesin = row.to_int16(5);
        }
        if (row.has_value(6))
        {
            this->falseness = row.to_int16(6);
        }
        this->heathenism = ((Type80)(row.to_int32(7)));
        this->SetKey(this->laundrymen, this->heathenism);
    }
    
    Table204Table::Table204Table()
    {
    }
    
    Table204Table::Table204Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table204Table::~Table204Table()
    {
    }
    
    void* Table204Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table204Row(row, ((Table204Table*)(table)));
    }
    
    const Table204Row* Table204Table::Find(short laundrymen, Type80 heathenism) const
    {
        return this->FindRow(laundrymen, heathenism);
    }
    
    Table_CarolRow::Table_CarolRow(CremaReader::irow& row, Table_CarolTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lumberyard = ((Type6)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Carl = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->firearm = row.to_boolean(2);
        }
        this->uncap = row.to_int8(3);
        if (row.has_value(4))
        {
            this->chairwoman = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->Bassett = ((Type8)(row.to_int32(5)));
        }
        this->SetKey(this->lumberyard, this->uncap);
    }
    
    Table_CarolTable::Table_CarolTable()
    {
    }
    
    Table_CarolTable::Table_CarolTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_CarolTable::~Table_CarolTable()
    {
    }
    
    void* Table_CarolTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_CarolRow(row, ((Table_CarolTable*)(table)));
    }
    
    const Table_CarolRow* Table_CarolTable::Find(Type6 lumberyard, char uncap) const
    {
        return this->FindRow(lumberyard, uncap);
    }
    
    Table141Row::Table141Row(CremaReader::irow& row, Table141Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->known = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->MacIntosh = row.to_single(1);
        }
        this->SetKey(this->known);
    }
    
    Table141Table::Table141Table()
    {
    }
    
    Table141Table::Table141Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table141Table::~Table141Table()
    {
    }
    
    void* Table141Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table141Row(row, ((Table141Table*)(table)));
    }
    
    const Table141Row* Table141Table::Find(bool known) const
    {
        return this->FindRow(known);
    }
    
    Table40Child1Row::Table40Child1Row(CremaReader::irow& row, Table40Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Eden = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->later = row.to_uint8(1);
        }
        this->SetKey(this->Eden);
    }
    
    Table40Child1Table::Table40Child1Table()
    {
    }
    
    Table40Child1Table::Table40Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table40Child1Table::Table40Child1Table(std::vector<Table40Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table40Child1Table::~Table40Child1Table()
    {
    }
    
    void* Table40Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table40Child1Row(row, ((Table40Child1Table*)(table)));
    }
    
    const Table40Child1Row* Table40Child1Table::Find(unsigned short Eden) const
    {
        return this->FindRow(Eden);
    }
    
    Table40Child1Table Table40Row::Child1Empty;
    
    Table40Row::Table40Row(CremaReader::irow& row, Table40Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->tenting = row.to_int16(0);
        if (row.has_value(1))
        {
            this->Alia = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->fajitas = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->endeavor = row.to_uint8(3);
        }
        if (row.has_value(4))
        {
            this->sensuous = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->tranquilized = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->beautifier = row.to_string(6);
        }
        if (row.has_value(7))
        {
            this->sulfurous = row.to_uint32(7);
        }
        if (row.has_value(8))
        {
            this->portrayer = row.to_boolean(8);
        }
        if (row.has_value(9))
        {
            this->compiler = ((Type_Attn)(row.to_int32(9)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table40Row::Child1Empty);
        }
        this->SetKey(this->tenting);
    }
    
    void Table40SetChild1(Table40Row* target, const std::vector<Table40Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table40Child1Table(childs);
    }
    
    Table40Table::Table40Table()
    {
    }
    
    Table40Table::Table40Table(CremaReader::itable& table)
    {
        this->Child1 = new Table40Child1Table(table.dataset().tables()["Table40.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table40SetChild1);
    }
    
    Table40Table::~Table40Table()
    {
        delete this->Child1;
    }
    
    void* Table40Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table40Row(row, ((Table40Table*)(table)));
    }
    
    const Table40Row* Table40Table::Find(short tenting) const
    {
        return this->FindRow(tenting);
    }
    
    Table57Row::Table57Row(CremaReader::irow& row, Table57Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->spanielled = row.to_double(0);
        if (row.has_value(1))
        {
            this->Isabelle = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->Camella = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->prodded = row.to_int16(3);
        }
        if (row.has_value(4))
        {
            this->coalescence = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->psychotherapist = row.to_datetime(5);
        }
        this->SetKey(this->spanielled);
    }
    
    Table57Table::Table57Table()
    {
    }
    
    Table57Table::Table57Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table57Table::~Table57Table()
    {
    }
    
    void* Table57Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table57Row(row, ((Table57Table*)(table)));
    }
    
    const Table57Row* Table57Table::Find(double spanielled) const
    {
        return this->FindRow(spanielled);
    }
    
    Table206Row::Table206Row(CremaReader::irow& row, Table206Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->homelessness = row.to_int64(0);
        if (row.has_value(1))
        {
            this->centering = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->porthole = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->giveaway = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->Klee = row.to_int16(4);
        }
        this->SetKey(this->homelessness);
    }
    
    Table206Table::Table206Table()
    {
    }
    
    Table206Table::Table206Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table206Table::~Table206Table()
    {
    }
    
    void* Table206Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table206Row(row, ((Table206Table*)(table)));
    }
    
    const Table206Row* Table206Table::Find(long long homelessness) const
    {
        return this->FindRow(homelessness);
    }
    
    Table_conicalRow::Table_conicalRow(CremaReader::irow& row, Table_conicalTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->narcoleptic = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->wakefulness = ((Type_canted)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->mealiness = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Miguela = ((Type_farinaceous)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->bedstead = ((Type_canted)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->hitherto = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->Ibsen = row.to_string(6);
        }
        if (row.has_value(7))
        {
            this->Shavuot = row.to_datetime(7);
        }
        if (row.has_value(8))
        {
            this->hedgerow = row.to_datetime(8);
        }
        if (row.has_value(9))
        {
            this->video = ((Type_seasonably)(row.to_int32(9)));
        }
        if (row.has_value(10))
        {
            this->Jemmy = row.to_uint16(10);
        }
        if (row.has_value(11))
        {
            this->Kublai = row.to_uint16(11);
        }
        if (row.has_value(12))
        {
            this->newsworthiness = ((Type21)(row.to_int32(12)));
        }
        if (row.has_value(13))
        {
            this->Bowery = row.to_int32(13);
        }
        this->devour = ((Type21)(row.to_int32(14)));
        this->SetKey(this->narcoleptic, this->devour);
    }
    
    Table_conicalTable::Table_conicalTable()
    {
    }
    
    Table_conicalTable::Table_conicalTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_conicalTable::~Table_conicalTable()
    {
    }
    
    void* Table_conicalTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_conicalRow(row, ((Table_conicalTable*)(table)));
    }
    
    const Table_conicalRow* Table_conicalTable::Find(unsigned int narcoleptic, Type21 devour) const
    {
        return this->FindRow(narcoleptic, devour);
    }
    
    Table22Row::Table22Row(CremaReader::irow& row, Table22Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->immediacy = row.to_int32(0);
        if (row.has_value(1))
        {
            this->dateline = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->setup = ((Type_Gretta)(row.to_int32(2)));
        }
        this->lettering = row.to_double(3);
        this->colatitude = ((Type_Multan)(row.to_int32(4)));
        if (row.has_value(5))
        {
            this->Sam = row.to_int8(5);
        }
        this->maleficence = row.to_single(6);
        if (row.has_value(7))
        {
            this->Fermi = ((Type1)(row.to_int32(7)));
        }
        this->SetKey(this->immediacy, this->lettering, this->colatitude, this->maleficence);
    }
    
    Table22Table::Table22Table()
    {
    }
    
    Table22Table::Table22Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table22Table::~Table22Table()
    {
    }
    
    void* Table22Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table22Row(row, ((Table22Table*)(table)));
    }
    
    const Table22Row* Table22Table::Find(int immediacy, double lettering, Type_Multan colatitude, float maleficence) const
    {
        return this->FindRow(immediacy, lettering, colatitude, maleficence);
    }
    
    Table_hydrosphereRow::Table_hydrosphereRow(CremaReader::irow& row, Table_hydrosphereTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Lehigh = ((Type11)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Attic = row.to_datetime(1);
        }
        this->malaria = row.to_duration(2);
        if (row.has_value(3))
        {
            this->Chiarra = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->Pound = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->prescient = row.to_datetime(5);
        }
        if (row.has_value(6))
        {
            this->infarction = row.to_int16(6);
        }
        this->SetKey(this->Lehigh, this->malaria);
    }
    
    Table_hydrosphereTable::Table_hydrosphereTable()
    {
    }
    
    Table_hydrosphereTable::Table_hydrosphereTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_hydrosphereTable::~Table_hydrosphereTable()
    {
    }
    
    void* Table_hydrosphereTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_hydrosphereRow(row, ((Table_hydrosphereTable*)(table)));
    }
    
    const Table_hydrosphereRow* Table_hydrosphereTable::Find(Type11 Lehigh, int malaria) const
    {
        return this->FindRow(Lehigh, malaria);
    }
    
    Table46Row::Table46Row(CremaReader::irow& row, Table46Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Sinclare = ((Type_applejack)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->superheat = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->theosophical = row.to_uint16(2);
        }
        if (row.has_value(3))
        {
            this->Nerta = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->Drugi = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->bemused = row.to_duration(5);
        }
        this->SetKey(this->Sinclare);
    }
    
    Table46Table::Table46Table()
    {
    }
    
    Table46Table::Table46Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table46Table::~Table46Table()
    {
    }
    
    void* Table46Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table46Row(row, ((Table46Table*)(table)));
    }
    
    const Table46Row* Table46Table::Find(Type_applejack Sinclare) const
    {
        return this->FindRow(Sinclare);
    }
    
    Table_agapaeRow::Table_agapaeRow(CremaReader::irow& row, Table_agapaeTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->pelvic = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->ephemera = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->paramecia = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->momentousness = ((Type_empathize)(row.to_int32(3)));
        }
        this->SetKey(this->pelvic);
    }
    
    Table_agapaeTable::Table_agapaeTable()
    {
    }
    
    Table_agapaeTable::Table_agapaeTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_agapaeTable::~Table_agapaeTable()
    {
    }
    
    void* Table_agapaeTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_agapaeRow(row, ((Table_agapaeTable*)(table)));
    }
    
    const Table_agapaeRow* Table_agapaeTable::Find(unsigned int pelvic) const
    {
        return this->FindRow(pelvic);
    }
    
    Table10Child_KerenskyRow::Table10Child_KerenskyRow(CremaReader::irow& row, Table10Child_KerenskyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->budgerigar = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->Ricca = row.to_duration(1);
        }
        this->Shepherd = ((Type_Arlan)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->opacity = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->donutted = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->cobwebbed = row.to_datetime(5);
        }
        if (row.has_value(6))
        {
            this->Irv = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->Veronica = ((Type_Arlan)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->argue = ((Type_canted)(row.to_int32(8)));
        }
        if (row.has_value(9))
        {
            this->conservancy = ((Type_Attn)(row.to_int32(9)));
        }
        this->SetKey(this->budgerigar, this->Shepherd);
    }
    
    Table10Child_KerenskyTable::Table10Child_KerenskyTable()
    {
    }
    
    Table10Child_KerenskyTable::Table10Child_KerenskyTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table10Child_KerenskyTable::Table10Child_KerenskyTable(std::vector<Table10Child_KerenskyRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table10Child_KerenskyTable::~Table10Child_KerenskyTable()
    {
    }
    
    void* Table10Child_KerenskyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table10Child_KerenskyRow(row, ((Table10Child_KerenskyTable*)(table)));
    }
    
    const Table10Child_KerenskyRow* Table10Child_KerenskyTable::Find(time_t budgerigar, Type_Arlan Shepherd) const
    {
        return this->FindRow(budgerigar, Shepherd);
    }
    
    Table10Child_KerenskyTable Table10Row::Child_KerenskyEmpty;
    
    Table10Row::Table10Row(CremaReader::irow& row, Table10Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->tortoiseshell = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->airfield = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->uneducated = ((Type_Multan)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->pickup = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->vantage = row.to_uint64(4);
        }
        if (row.has_value(5))
        {
            this->syllabus = row.to_uint64(5);
        }
        if ((this->Child_Kerensky == nullptr))
        {
            this->Child_Kerensky = &(Table10Row::Child_KerenskyEmpty);
        }
        this->SetKey(this->tortoiseshell);
    }
    
    void Table10SetChild_Kerensky(Table10Row* target, const std::vector<Table10Child_KerenskyRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Kerensky = new Table10Child_KerenskyTable(childs);
    }
    
    Table10Table::Table10Table()
    {
    }
    
    Table10Table::Table10Table(CremaReader::itable& table)
    {
        this->Child_Kerensky = new Table10Child_KerenskyTable(table.dataset().tables()["Table10.Child_Kerensky"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Kerensky->Rows, Table10SetChild_Kerensky);
    }
    
    Table10Table::~Table10Table()
    {
        delete this->Child_Kerensky;
    }
    
    void* Table10Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table10Row(row, ((Table10Table*)(table)));
    }
    
    const Table10Row* Table10Table::Find(unsigned char tortoiseshell) const
    {
        return this->FindRow(tortoiseshell);
    }
    
    Table_CarolanChild1Row::Table_CarolanChild1Row(CremaReader::irow& row, Table_CarolanChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->simulcast = row.to_duration(0);
        this->SetKey(this->simulcast);
    }
    
    Table_CarolanChild1Table::Table_CarolanChild1Table()
    {
    }
    
    Table_CarolanChild1Table::Table_CarolanChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_CarolanChild1Table::Table_CarolanChild1Table(std::vector<Table_CarolanChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_CarolanChild1Table::~Table_CarolanChild1Table()
    {
    }
    
    void* Table_CarolanChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_CarolanChild1Row(row, ((Table_CarolanChild1Table*)(table)));
    }
    
    const Table_CarolanChild1Row* Table_CarolanChild1Table::Find(int simulcast) const
    {
        return this->FindRow(simulcast);
    }
    
    Table_CarolanChild2Row::Table_CarolanChild2Row(CremaReader::irow& row, Table_CarolanChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->proletarianization = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->given = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->Tierney = row.to_string(2);
        }
        this->Binghamton = ((Type11)(row.to_int32(3)));
        if (row.has_value(4))
        {
            this->biggish = row.to_boolean(4);
        }
        this->SetKey(this->proletarianization, this->Binghamton);
    }
    
    Table_CarolanChild2Table::Table_CarolanChild2Table()
    {
    }
    
    Table_CarolanChild2Table::Table_CarolanChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_CarolanChild2Table::Table_CarolanChild2Table(std::vector<Table_CarolanChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_CarolanChild2Table::~Table_CarolanChild2Table()
    {
    }
    
    void* Table_CarolanChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_CarolanChild2Row(row, ((Table_CarolanChild2Table*)(table)));
    }
    
    const Table_CarolanChild2Row* Table_CarolanChild2Table::Find(time_t proletarianization, Type11 Binghamton) const
    {
        return this->FindRow(proletarianization, Binghamton);
    }
    
    Table_CarolanChild1Table Table_CarolanRow::Child1Empty;
    
    Table_CarolanChild2Table Table_CarolanRow::Child2Empty;
    
    Table_CarolanRow::Table_CarolanRow(CremaReader::irow& row, Table_CarolanTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->adviser = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Peterson = ((Type_pledge)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->colleague = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->cumuli = ((Type24)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->analyzed = row.to_int32(4);
        }
        if (row.has_value(5))
        {
            this->scowler = row.to_int64(5);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_CarolanRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_CarolanRow::Child2Empty);
        }
        this->SetKey(this->adviser);
    }
    
    void Table_CarolanSetChild1(Table_CarolanRow* target, const std::vector<Table_CarolanChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_CarolanChild1Table(childs);
    }
    
    void Table_CarolanSetChild2(Table_CarolanRow* target, const std::vector<Table_CarolanChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_CarolanChild2Table(childs);
    }
    
    Table_CarolanTable::Table_CarolanTable()
    {
    }
    
    Table_CarolanTable::Table_CarolanTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_CarolanChild1Table(table.dataset().tables()["Table_Carolan.Child1"]);
        this->Child2 = new Table_CarolanChild2Table(table.dataset().tables()["Table_Carolan.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_CarolanSetChild1);
        this->SetRelations(this->Child2->Rows, Table_CarolanSetChild2);
    }
    
    Table_CarolanTable::~Table_CarolanTable()
    {
        delete this->Child1;
        delete this->Child2;
    }
    
    void* Table_CarolanTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_CarolanRow(row, ((Table_CarolanTable*)(table)));
    }
    
    const Table_CarolanRow* Table_CarolanTable::Find(unsigned short adviser) const
    {
        return this->FindRow(adviser);
    }
    
    Table_repressivenessRow::Table_repressivenessRow(CremaReader::irow& row, Table_repressivenessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->payed = row.to_int16(0);
        if (row.has_value(1))
        {
            this->Neville = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->loom = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->laxativeness = row.to_string(3);
        }
        this->SetKey(this->payed);
    }
    
    Table_repressivenessTable::Table_repressivenessTable()
    {
    }
    
    Table_repressivenessTable::Table_repressivenessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_repressivenessTable::~Table_repressivenessTable()
    {
    }
    
    void* Table_repressivenessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_repressivenessRow(row, ((Table_repressivenessTable*)(table)));
    }
    
    const Table_repressivenessRow* Table_repressivenessTable::Find(short payed) const
    {
        return this->FindRow(payed);
    }
    
    Table183Row::Table183Row(CremaReader::irow& row, Table183Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->jumpily = ((Type44)(row.to_int32(0)));
        this->SetKey(this->jumpily);
    }
    
    Table183Table::Table183Table()
    {
    }
    
    Table183Table::Table183Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table183Table::~Table183Table()
    {
    }
    
    void* Table183Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table183Row(row, ((Table183Table*)(table)));
    }
    
    const Table183Row* Table183Table::Find(Type44 jumpily) const
    {
        return this->FindRow(jumpily);
    }
    
    Table94Row::Table94Row(CremaReader::irow& row, Table94Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->dialogged = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->goodwill = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->pet = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Indianian = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->exec = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->drubber = row.to_single(5);
        }
        this->SetKey(this->dialogged);
    }
    
    Table94Table::Table94Table()
    {
    }
    
    Table94Table::Table94Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table94Table::~Table94Table()
    {
    }
    
    void* Table94Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table94Row(row, ((Table94Table*)(table)));
    }
    
    const Table94Row* Table94Table::Find(unsigned int dialogged) const
    {
        return this->FindRow(dialogged);
    }
    
    Table82Row::Table82Row(CremaReader::irow& row, Table82Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->bade = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->spacier = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->please = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->$case = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->Wilhelmina = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->decor = ((Type_primitiveness)(row.to_int32(5)));
        }
        this->SetKey(this->bade);
    }
    
    Table82Table::Table82Table()
    {
    }
    
    Table82Table::Table82Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table82Table::~Table82Table()
    {
    }
    
    void* Table82Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table82Row(row, ((Table82Table*)(table)));
    }
    
    const Table82Row* Table82Table::Find(unsigned short bade) const
    {
        return this->FindRow(bade);
    }
    
    Table87Child1Row::Table87Child1Row(CremaReader::irow& row, Table87Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Giff = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Lewiss = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->graveyard = ((Type_Multan)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->organism = row.to_uint32(3);
        }
        this->heather = row.to_int64(4);
        if (row.has_value(5))
        {
            this->Kyla = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->prognostication = row.to_uint16(6);
        }
        if (row.has_value(7))
        {
            this->mesquite = ((Type_Madison)(row.to_int32(7)));
        }
        this->SetKey(this->Giff, this->heather);
    }
    
    Table87Child1Table::Table87Child1Table()
    {
    }
    
    Table87Child1Table::Table87Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table87Child1Table::Table87Child1Table(std::vector<Table87Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table87Child1Table::~Table87Child1Table()
    {
    }
    
    void* Table87Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table87Child1Row(row, ((Table87Child1Table*)(table)));
    }
    
    const Table87Child1Row* Table87Child1Table::Find(int Giff, long long heather) const
    {
        return this->FindRow(Giff, heather);
    }
    
    Table87Child1Table Table87Row::Child1Empty;
    
    Table87Row::Table87Row(CremaReader::irow& row, Table87Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->flabbily = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->wasteful = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->litigiousness = row.to_uint64(2);
        }
        if (row.has_value(3))
        {
            this->chasm = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->Malena = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->admirable = ((Type_canted)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Thatcher = row.to_boolean(6);
        }
        if (row.has_value(7))
        {
            this->prowler = row.to_int32(7);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table87Row::Child1Empty);
        }
        this->SetKey(this->flabbily);
    }
    
    void Table87SetChild1(Table87Row* target, const std::vector<Table87Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table87Child1Table(childs);
    }
    
    Table87Table::Table87Table()
    {
    }
    
    Table87Table::Table87Table(CremaReader::itable& table)
    {
        this->Child1 = new Table87Child1Table(table.dataset().tables()["Table87.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table87SetChild1);
    }
    
    Table87Table::~Table87Table()
    {
        delete this->Child1;
    }
    
    void* Table87Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table87Row(row, ((Table87Table*)(table)));
    }
    
    const Table87Row* Table87Table::Find(time_t flabbily) const
    {
        return this->FindRow(flabbily);
    }
    
    Table_implacablenessRow::Table_implacablenessRow(CremaReader::irow& row, Table_implacablenessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Tanny = row.to_double(0);
        if (row.has_value(1))
        {
            this->locale = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->fagoting = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->aim = ((Type_Madison)(row.to_int32(3)));
        }
        this->brutality = row.to_string(4);
        if (row.has_value(5))
        {
            this->scantly = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->British = row.to_int32(6);
        }
        if (row.has_value(7))
        {
            this->broadcast = row.to_int8(7);
        }
        if (row.has_value(8))
        {
            this->injurer = row.to_single(8);
        }
        this->SetKey(this->Tanny, this->brutality);
    }
    
    Table_implacablenessTable::Table_implacablenessTable()
    {
    }
    
    Table_implacablenessTable::Table_implacablenessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_implacablenessTable::~Table_implacablenessTable()
    {
    }
    
    void* Table_implacablenessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_implacablenessRow(row, ((Table_implacablenessTable*)(table)));
    }
    
    const Table_implacablenessRow* Table_implacablenessTable::Find(double Tanny, const std::string& brutality) const
    {
        return this->FindRow(Tanny, brutality);
    }
    
    Table_fleeingRow::Table_fleeingRow(CremaReader::irow& row, Table_fleeingTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->corpsman = row.to_double(0);
        if (row.has_value(1))
        {
            this->Araucanian = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Kaposi = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->hyperemia = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->pensiveness = row.to_single(4);
        }
        this->jetting = row.to_boolean(5);
        if (row.has_value(6))
        {
            this->babe = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->clears = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->codetermine = row.to_duration(8);
        }
        this->SetKey(this->corpsman, this->jetting);
    }
    
    Table_fleeingTable::Table_fleeingTable()
    {
    }
    
    Table_fleeingTable::Table_fleeingTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_fleeingTable::~Table_fleeingTable()
    {
    }
    
    void* Table_fleeingTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_fleeingRow(row, ((Table_fleeingTable*)(table)));
    }
    
    const Table_fleeingRow* Table_fleeingTable::Find(double corpsman, bool jetting) const
    {
        return this->FindRow(corpsman, jetting);
    }
    
    Table50Row::Table50Row(CremaReader::irow& row, Table50Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->travelogue = row.to_single(0);
        if (row.has_value(1))
        {
            this->mercurial = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->divider = row.to_uint16(2);
        }
        if (row.has_value(3))
        {
            this->schmuck = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->epistemic = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->Flin = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->Gaulish = row.to_datetime(6);
        }
        if (row.has_value(7))
        {
            this->apportionment = row.to_string(7);
        }
        if (row.has_value(8))
        {
            this->toxin = ((Type_Arlan)(row.to_int32(8)));
        }
        this->SetKey(this->travelogue);
    }
    
    Table50Table::Table50Table()
    {
    }
    
    Table50Table::Table50Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table50Table::~Table50Table()
    {
    }
    
    void* Table50Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table50Row(row, ((Table50Table*)(table)));
    }
    
    const Table50Row* Table50Table::Find(float travelogue) const
    {
        return this->FindRow(travelogue);
    }
    
    Table53Row::Table53Row(CremaReader::irow& row, Table53Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->collocate = row.to_int64(0);
        if (row.has_value(1))
        {
            this->dependable = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->wetback = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->primitive = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->theme = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->Ladonna = row.to_int64(5);
        }
        if (row.has_value(6))
        {
            this->desk = ((Type_Dianna)(row.to_int32(6)));
        }
        this->SetKey(this->collocate);
    }
    
    Table53Table::Table53Table()
    {
    }
    
    Table53Table::Table53Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table53Table::~Table53Table()
    {
    }
    
    void* Table53Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table53Row(row, ((Table53Table*)(table)));
    }
    
    const Table53Row* Table53Table::Find(long long collocate) const
    {
        return this->FindRow(collocate);
    }
    
    Table89Row::Table89Row(CremaReader::irow& row, Table89Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->rot = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Stradivari = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->extemporizer = ((Type_Meiji)(row.to_int32(2)));
        }
        this->periphrases = ((Type_artiness)(row.to_int32(3)));
        if (row.has_value(4))
        {
            this->whisperer = row.to_uint16(4);
        }
        this->Warhol = row.to_double(5);
        if (row.has_value(6))
        {
            this->oak = ((Type30)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->propagandize = ((Type60)(row.to_int32(7)));
        }
        this->SetKey(this->rot, this->periphrases, this->Warhol);
    }
    
    Table89Table::Table89Table()
    {
    }
    
    Table89Table::Table89Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table89Table::~Table89Table()
    {
    }
    
    void* Table89Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table89Row(row, ((Table89Table*)(table)));
    }
    
    const Table89Row* Table89Table::Find(char rot, Type_artiness periphrases, double Warhol) const
    {
        return this->FindRow(rot, periphrases, Warhol);
    }
    
    Table109Row::Table109Row(CremaReader::irow& row, Table109Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->flourisher = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->Englishmen = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->registrar = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->flimflammed = row.to_uint16(3);
        }
        if (row.has_value(4))
        {
            this->Victoria = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->outgrip = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->Tootsie = row.to_uint64(6);
        }
        if (row.has_value(7))
        {
            this->monogamist = row.to_int32(7);
        }
        if (row.has_value(8))
        {
            this->PIN = ((Type16)(row.to_int32(8)));
        }
        this->SetKey(this->flourisher);
    }
    
    Table109Table::Table109Table()
    {
    }
    
    Table109Table::Table109Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table109Table::~Table109Table()
    {
    }
    
    void* Table109Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table109Row(row, ((Table109Table*)(table)));
    }
    
    const Table109Row* Table109Table::Find(unsigned int flourisher) const
    {
        return this->FindRow(flourisher);
    }
    
    Table180Row::Table180Row(CremaReader::irow& row, Table180Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->boa = row.to_int32(0);
        if (row.has_value(1))
        {
            this->esp = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->Minerva = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->centrality = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->thrummed = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->throwback = row.to_double(5);
        }
        this->SetKey(this->boa);
    }
    
    Table180Table::Table180Table()
    {
    }
    
    Table180Table::Table180Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table180Table::~Table180Table()
    {
    }
    
    void* Table180Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table180Row(row, ((Table180Table*)(table)));
    }
    
    const Table180Row* Table180Table::Find(int boa) const
    {
        return this->FindRow(boa);
    }
    
    Table20Child1Row::Table20Child1Row(CremaReader::irow& row, Table20Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->floodlit = row.to_int32(0);
        if (row.has_value(1))
        {
            this->hokiest = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->grouchily = row.to_int64(2);
        }
        this->SetKey(this->floodlit);
    }
    
    Table20Child1Table::Table20Child1Table()
    {
    }
    
    Table20Child1Table::Table20Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table20Child1Table::Table20Child1Table(std::vector<Table20Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table20Child1Table::~Table20Child1Table()
    {
    }
    
    void* Table20Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table20Child1Row(row, ((Table20Child1Table*)(table)));
    }
    
    const Table20Child1Row* Table20Child1Table::Find(int floodlit) const
    {
        return this->FindRow(floodlit);
    }
    
    Table20Child2Row::Table20Child2Row(CremaReader::irow& row, Table20Child2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Toni = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->solidarity = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->outgrowths = row.to_uint16(2);
        }
        if (row.has_value(3))
        {
            this->Bengali = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->contrive = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->icebound = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->nubbin = row.to_uint16(6);
        }
        if (row.has_value(7))
        {
            this->bacteriology = ((Type50)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->Jainism = row.to_string(8);
        }
        if (row.has_value(9))
        {
            this->urinal = row.to_int16(9);
        }
        if (row.has_value(10))
        {
            this->Maguire = row.to_boolean(10);
        }
        if (row.has_value(11))
        {
            this->chainsaw = row.to_duration(11);
        }
        this->SetKey(this->Toni);
    }
    
    Table20Child2Table::Table20Child2Table()
    {
    }
    
    Table20Child2Table::Table20Child2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table20Child2Table::Table20Child2Table(std::vector<Table20Child2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table20Child2Table::~Table20Child2Table()
    {
    }
    
    void* Table20Child2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table20Child2Row(row, ((Table20Child2Table*)(table)));
    }
    
    const Table20Child2Row* Table20Child2Table::Find(time_t Toni) const
    {
        return this->FindRow(Toni);
    }
    
    Table20Child1Table Table20Row::Child1Empty;
    
    Table20Child2Table Table20Row::Child2Empty;
    
    Table20Row::Table20Row(CremaReader::irow& row, Table20Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->flaxseed = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->Mithra = ((Type_RhodesDeletable)(row.to_int32(1)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table20Row::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table20Row::Child2Empty);
        }
        this->SetKey(this->flaxseed);
    }
    
    void Table20SetChild1(Table20Row* target, const std::vector<Table20Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table20Child1Table(childs);
    }
    
    void Table20SetChild2(Table20Row* target, const std::vector<Table20Child2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table20Child2Table(childs);
    }
    
    Table20Table::Table20Table()
    {
    }
    
    Table20Table::Table20Table(CremaReader::itable& table)
    {
        this->Child1 = new Table20Child1Table(table.dataset().tables()["Table20.Child1"]);
        this->Child2 = new Table20Child2Table(table.dataset().tables()["Table20.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table20SetChild1);
        this->SetRelations(this->Child2->Rows, Table20SetChild2);
    }
    
    Table20Table::~Table20Table()
    {
        delete this->Child1;
        delete this->Child2;
    }
    
    void* Table20Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table20Row(row, ((Table20Table*)(table)));
    }
    
    const Table20Row* Table20Table::Find(unsigned char flaxseed) const
    {
        return this->FindRow(flaxseed);
    }
    
    Table21Child1Row::Table21Child1Row(CremaReader::irow& row, Table21Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->parabolic = row.to_int32(0);
        if (row.has_value(1))
        {
            this->syncopation = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->steamroller = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->purloiner = row.to_int64(3);
        }
        this->rattrap = row.to_int16(4);
        if (row.has_value(5))
        {
            this->Redondo = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->peach = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->asbestos = row.to_int64(7);
        }
        if (row.has_value(8))
        {
            this->glazing = row.to_uint16(8);
        }
        this->SetKey(this->parabolic, this->rattrap);
    }
    
    Table21Child1Table::Table21Child1Table()
    {
    }
    
    Table21Child1Table::Table21Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table21Child1Table::Table21Child1Table(std::vector<Table21Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table21Child1Table::~Table21Child1Table()
    {
    }
    
    void* Table21Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table21Child1Row(row, ((Table21Child1Table*)(table)));
    }
    
    const Table21Child1Row* Table21Child1Table::Find(int parabolic, short rattrap) const
    {
        return this->FindRow(parabolic, rattrap);
    }
    
    Table21Child1Table Table21Row::Child1Empty;
    
    Table21Row::Table21Row(CremaReader::irow& row, Table21Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->marginalia = row.to_uint64(0);
        this->chatted = row.to_int64(1);
        if (row.has_value(2))
        {
            this->dutiful = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->strangulate = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->Rozella = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->patrician = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->brunet = row.to_uint16(6);
        }
        if (row.has_value(7))
        {
            this->sweeping = row.to_int16(7);
        }
        if (row.has_value(8))
        {
            this->priesthood = row.to_int32(8);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table21Row::Child1Empty);
        }
        this->SetKey(this->marginalia, this->chatted);
    }
    
    void Table21SetChild1(Table21Row* target, const std::vector<Table21Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table21Child1Table(childs);
    }
    
    Table21Table::Table21Table()
    {
    }
    
    Table21Table::Table21Table(CremaReader::itable& table)
    {
        this->Child1 = new Table21Child1Table(table.dataset().tables()["Table21.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table21SetChild1);
    }
    
    Table21Table::~Table21Table()
    {
        delete this->Child1;
    }
    
    void* Table21Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table21Row(row, ((Table21Table*)(table)));
    }
    
    const Table21Row* Table21Table::Find(unsigned long long marginalia, long long chatted) const
    {
        return this->FindRow(marginalia, chatted);
    }
    
    Table43Child2Row::Table43Child2Row(CremaReader::irow& row, Table43Child2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->convention = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->Legree = row.to_double(1);
        }
        this->SetKey(this->convention);
    }
    
    Table43Child2Table::Table43Child2Table()
    {
    }
    
    Table43Child2Table::Table43Child2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table43Child2Table::Table43Child2Table(std::vector<Table43Child2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table43Child2Table::~Table43Child2Table()
    {
    }
    
    void* Table43Child2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table43Child2Row(row, ((Table43Child2Table*)(table)));
    }
    
    const Table43Child2Row* Table43Child2Table::Find(time_t convention) const
    {
        return this->FindRow(convention);
    }
    
    Table43Child2Table Table43Row::Child2Empty;
    
    Table43Row::Table43Row(CremaReader::irow& row, Table43Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->foolery = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->valuable = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->flood = row.to_datetime(2);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table43Row::Child2Empty);
        }
        this->SetKey(this->foolery);
    }
    
    void Table43SetChild2(Table43Row* target, const std::vector<Table43Child2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table43Child2Table(childs);
    }
    
    Table43Table::Table43Table()
    {
    }
    
    Table43Table::Table43Table(CremaReader::itable& table)
    {
        this->Child2 = new Table43Child2Table(table.dataset().tables()["Table43.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child2->Rows, Table43SetChild2);
    }
    
    Table43Table::~Table43Table()
    {
        delete this->Child2;
    }
    
    void* Table43Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table43Row(row, ((Table43Table*)(table)));
    }
    
    const Table43Row* Table43Table::Find(unsigned int foolery) const
    {
        return this->FindRow(foolery);
    }
    
    Table51Row::Table51Row(CremaReader::irow& row, Table51Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Louise = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->fecal = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->humanoid = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->Thaddus = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->Pollux = row.to_uint8(4);
        }
        this->SetKey(this->Louise);
    }
    
    Table51Table::Table51Table()
    {
    }
    
    Table51Table::Table51Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table51Table::~Table51Table()
    {
    }
    
    void* Table51Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table51Row(row, ((Table51Table*)(table)));
    }
    
    const Table51Row* Table51Table::Find(unsigned long long Louise) const
    {
        return this->FindRow(Louise);
    }
    
    Table108Row::Table108Row(CremaReader::irow& row, Table108Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Latin = row.to_int32(0);
        if (row.has_value(1))
        {
            this->inhalant = row.to_single(1);
        }
        this->curling = row.to_uint32(2);
        if (row.has_value(3))
        {
            this->soliloquize = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->Minna = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->parader = ((Type_gustily)(row.to_int32(5)));
        }
        this->SetKey(this->Latin, this->curling);
    }
    
    Table108Table::Table108Table()
    {
    }
    
    Table108Table::Table108Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table108Table::~Table108Table()
    {
    }
    
    void* Table108Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table108Row(row, ((Table108Table*)(table)));
    }
    
    const Table108Row* Table108Table::Find(int Latin, unsigned int curling) const
    {
        return this->FindRow(Latin, curling);
    }
    
    Table76Row::Table76Row(CremaReader::irow& row, Table76Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Honoria = row.to_int64(0);
        this->Joyce = ((Type21)(row.to_int32(1)));
        if (row.has_value(2))
        {
            this->gloaming = row.to_int64(2);
        }
        this->archway = ((Type_Dianna)(row.to_int32(3)));
        if (row.has_value(4))
        {
            this->hookedness = ((Type_Madison)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->dichotomous = row.to_datetime(5);
        }
        if (row.has_value(6))
        {
            this->tessellation = row.to_single(6);
        }
        if (row.has_value(7))
        {
            this->chime = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->Zhukov = row.to_int64(8);
        }
        if (row.has_value(9))
        {
            this->Romulus = row.to_double(9);
        }
        if (row.has_value(10))
        {
            this->liquidizer = row.to_uint32(10);
        }
        this->RSV = row.to_int64(11);
        this->chairwoman = row.to_int64(12);
        this->SetKey(this->Honoria, this->Joyce, this->archway, this->RSV, this->chairwoman);
    }
    
    Table76Table::Table76Table()
    {
    }
    
    Table76Table::Table76Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table76Table::~Table76Table()
    {
    }
    
    void* Table76Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table76Row(row, ((Table76Table*)(table)));
    }
    
    const Table76Row* Table76Table::Find(long long Honoria, Type21 Joyce, Type_Dianna archway, long long RSV, long long chairwoman) const
    {
        return this->FindRow(Honoria, Joyce, archway, RSV, chairwoman);
    }
    
    Table_duxesChild2Row::Table_duxesChild2Row(CremaReader::irow& row, Table_duxesChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->beret = row.to_double(0);
        this->Gussy = row.to_int32(1);
        this->SetKey(this->beret, this->Gussy);
    }
    
    Table_duxesChild2Table::Table_duxesChild2Table()
    {
    }
    
    Table_duxesChild2Table::Table_duxesChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_duxesChild2Table::Table_duxesChild2Table(std::vector<Table_duxesChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_duxesChild2Table::~Table_duxesChild2Table()
    {
    }
    
    void* Table_duxesChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_duxesChild2Row(row, ((Table_duxesChild2Table*)(table)));
    }
    
    const Table_duxesChild2Row* Table_duxesChild2Table::Find(double beret, int Gussy) const
    {
        return this->FindRow(beret, Gussy);
    }
    
    Table_duxesChild2Table Table_duxesRow::Child2Empty;
    
    Table_duxesRow::Table_duxesRow(CremaReader::irow& row, Table_duxesTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Argentina = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->wingding = ((Type_Multan)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->HUD = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->energize = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->sidewalk = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->underground = ((Type44)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Quakerism = row.to_duration(6);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_duxesRow::Child2Empty);
        }
        this->SetKey(this->Argentina);
    }
    
    void Table_duxesSetChild2(Table_duxesRow* target, const std::vector<Table_duxesChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_duxesChild2Table(childs);
    }
    
    Table_duxesTable::Table_duxesTable()
    {
    }
    
    Table_duxesTable::Table_duxesTable(CremaReader::itable& table)
    {
        this->Child2 = new Table_duxesChild2Table(table.dataset().tables()["Table_duxes.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child2->Rows, Table_duxesSetChild2);
    }
    
    Table_duxesTable::~Table_duxesTable()
    {
        delete this->Child2;
    }
    
    void* Table_duxesTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_duxesRow(row, ((Table_duxesTable*)(table)));
    }
    
    const Table_duxesRow* Table_duxesTable::Find(unsigned int Argentina) const
    {
        return this->FindRow(Argentina);
    }
    
    Table196Row::Table196Row(CremaReader::irow& row, Table196Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->chopped = row.to_int16(0);
        if (row.has_value(1))
        {
            this->chorister = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->overdue = ((Type49)(row.to_int32(2)));
        }
        this->signaling = ((Type88)(row.to_int32(3)));
        if (row.has_value(4))
        {
            this->surrender = ((Type22)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->need = ((Type51)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->nos = ((Type39)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->isothermal = row.to_uint16(7);
        }
        if (row.has_value(8))
        {
            this->id = row.to_datetime(8);
        }
        if (row.has_value(9))
        {
            this->Johnathon = ((Type_Jenelle)(row.to_int32(9)));
        }
        this->SetKey(this->chopped, this->signaling);
    }
    
    Table196Table::Table196Table()
    {
    }
    
    Table196Table::Table196Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table196Table::~Table196Table()
    {
    }
    
    void* Table196Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table196Row(row, ((Table196Table*)(table)));
    }
    
    const Table196Row* Table196Table::Find(short chopped, Type88 signaling) const
    {
        return this->FindRow(chopped, signaling);
    }
    
    Table150Row::Table150Row(CremaReader::irow& row, Table150Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->impenetrable = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->tonsillitis = row.to_uint16(1);
        }
        this->SetKey(this->impenetrable);
    }
    
    Table150Table::Table150Table()
    {
    }
    
    Table150Table::Table150Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table150Table::~Table150Table()
    {
    }
    
    void* Table150Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table150Row(row, ((Table150Table*)(table)));
    }
    
    const Table150Row* Table150Table::Find(unsigned short impenetrable) const
    {
        return this->FindRow(impenetrable);
    }
    
    Table_replenishRow::Table_replenishRow(CremaReader::irow& row, Table_replenishTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->unnavigable = row.to_single(0);
        if (row.has_value(1))
        {
            this->Janessa = row.to_uint64(1);
        }
        this->assiduity = row.to_datetime(2);
        if (row.has_value(3))
        {
            this->yardmaster = ((Type8)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->Sir = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->thermostat = ((Type_Attn)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->popinjay = row.to_string(6);
        }
        if (row.has_value(7))
        {
            this->planetarium = row.to_uint16(7);
        }
        if (row.has_value(8))
        {
            this->jasper = ((Type_insolent)(row.to_int32(8)));
        }
        this->SetKey(this->unnavigable, this->assiduity);
    }
    
    Table_replenishTable::Table_replenishTable()
    {
    }
    
    Table_replenishTable::Table_replenishTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_replenishTable::~Table_replenishTable()
    {
    }
    
    void* Table_replenishTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_replenishRow(row, ((Table_replenishTable*)(table)));
    }
    
    const Table_replenishRow* Table_replenishTable::Find(float unnavigable, time_t assiduity) const
    {
        return this->FindRow(unnavigable, assiduity);
    }
    
    Table_bedpanChild_admissionRow::Table_bedpanChild_admissionRow(CremaReader::irow& row, Table_bedpanChild_admissionTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Shackleton = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->austereness = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->casaba = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->reflection = row.to_boolean(3);
        }
        this->SetKey(this->Shackleton);
    }
    
    Table_bedpanChild_admissionTable::Table_bedpanChild_admissionTable()
    {
    }
    
    Table_bedpanChild_admissionTable::Table_bedpanChild_admissionTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_bedpanChild_admissionTable::Table_bedpanChild_admissionTable(std::vector<Table_bedpanChild_admissionRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_bedpanChild_admissionTable::~Table_bedpanChild_admissionTable()
    {
    }
    
    void* Table_bedpanChild_admissionTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_bedpanChild_admissionRow(row, ((Table_bedpanChild_admissionTable*)(table)));
    }
    
    const Table_bedpanChild_admissionRow* Table_bedpanChild_admissionTable::Find(unsigned long long Shackleton) const
    {
        return this->FindRow(Shackleton);
    }
    
    Table_bedpanChild1Row::Table_bedpanChild1Row(CremaReader::irow& row, Table_bedpanChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hamster = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->badinage = ((Type3)(row.to_int32(1)));
        }
        this->misogamist = ((Type33)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->anathema = row.to_uint8(3);
        }
        if (row.has_value(4))
        {
            this->Armin = row.to_int16(4);
        }
        if (row.has_value(5))
        {
            this->syllabusss = row.to_datetime(5);
        }
        this->SetKey(this->hamster, this->misogamist);
    }
    
    Table_bedpanChild1Table::Table_bedpanChild1Table()
    {
    }
    
    Table_bedpanChild1Table::Table_bedpanChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_bedpanChild1Table::Table_bedpanChild1Table(std::vector<Table_bedpanChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_bedpanChild1Table::~Table_bedpanChild1Table()
    {
    }
    
    void* Table_bedpanChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_bedpanChild1Row(row, ((Table_bedpanChild1Table*)(table)));
    }
    
    const Table_bedpanChild1Row* Table_bedpanChild1Table::Find(unsigned char hamster, Type33 misogamist) const
    {
        return this->FindRow(hamster, misogamist);
    }
    
    Table_bedpanChild_admissionTable Table_bedpanRow::Child_admissionEmpty;
    
    Table_bedpanChild1Table Table_bedpanRow::Child1Empty;
    
    Table_bedpanRow::Table_bedpanRow(CremaReader::irow& row, Table_bedpanTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Stendhal = row.to_datetime(0);
        this->cheerer = row.to_int32(1);
        if (row.has_value(2))
        {
            this->wreckage = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Andriette = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->oviduct = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->Osbourn = row.to_int64(5);
        }
        if (row.has_value(6))
        {
            this->laminae = row.to_int64(6);
        }
        if (row.has_value(7))
        {
            this->deferring = row.to_duration(7);
        }
        if (row.has_value(8))
        {
            this->straitness = row.to_boolean(8);
        }
        if ((this->Child_admission == nullptr))
        {
            this->Child_admission = &(Table_bedpanRow::Child_admissionEmpty);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_bedpanRow::Child1Empty);
        }
        this->SetKey(this->Stendhal, this->cheerer);
    }
    
    void Table_bedpanSetChild_admission(Table_bedpanRow* target, const std::vector<Table_bedpanChild_admissionRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_admission = new Table_bedpanChild_admissionTable(childs);
    }
    
    void Table_bedpanSetChild1(Table_bedpanRow* target, const std::vector<Table_bedpanChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_bedpanChild1Table(childs);
    }
    
    Table_bedpanTable::Table_bedpanTable()
    {
    }
    
    Table_bedpanTable::Table_bedpanTable(CremaReader::itable& table)
    {
        this->Child_admission = new Table_bedpanChild_admissionTable(table.dataset().tables()["Table_bedpan.Child_admission"]);
        this->Child1 = new Table_bedpanChild1Table(table.dataset().tables()["Table_bedpan.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_admission->Rows, Table_bedpanSetChild_admission);
        this->SetRelations(this->Child1->Rows, Table_bedpanSetChild1);
    }
    
    Table_bedpanTable::~Table_bedpanTable()
    {
        delete this->Child_admission;
        delete this->Child1;
    }
    
    void* Table_bedpanTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_bedpanRow(row, ((Table_bedpanTable*)(table)));
    }
    
    const Table_bedpanRow* Table_bedpanTable::Find(time_t Stendhal, int cheerer) const
    {
        return this->FindRow(Stendhal, cheerer);
    }
    
    Table_wristRow::Table_wristRow(CremaReader::irow& row, Table_wristTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Gerri = row.to_int16(0);
        if (row.has_value(1))
        {
            this->Alia = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->sensuous = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->beautifier = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->nugget = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->picketer = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->graceful = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->epic = row.to_int16(7);
        }
        this->SetKey(this->Gerri);
    }
    
    Table_wristTable::Table_wristTable()
    {
    }
    
    Table_wristTable::Table_wristTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_wristTable::~Table_wristTable()
    {
    }
    
    void* Table_wristTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_wristRow(row, ((Table_wristTable*)(table)));
    }
    
    const Table_wristRow* Table_wristTable::Find(short Gerri) const
    {
        return this->FindRow(Gerri);
    }
    
    Table33Child_dissociableRow::Table33Child_dissociableRow(CremaReader::irow& row, Table33Child_dissociableTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->highbrow = row.to_int32(0);
        if (row.has_value(1))
        {
            this->howsoever = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->Shoshanna = row.to_uint32(2);
        }
        this->SetKey(this->highbrow);
    }
    
    Table33Child_dissociableTable::Table33Child_dissociableTable()
    {
    }
    
    Table33Child_dissociableTable::Table33Child_dissociableTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table33Child_dissociableTable::Table33Child_dissociableTable(std::vector<Table33Child_dissociableRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table33Child_dissociableTable::~Table33Child_dissociableTable()
    {
    }
    
    void* Table33Child_dissociableTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table33Child_dissociableRow(row, ((Table33Child_dissociableTable*)(table)));
    }
    
    const Table33Child_dissociableRow* Table33Child_dissociableTable::Find(int highbrow) const
    {
        return this->FindRow(highbrow);
    }
    
    Table33Child_dissociableTable Table33Row::Child_dissociableEmpty;
    
    Table33Row::Table33Row(CremaReader::irow& row, Table33Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->meatpacking = row.to_double(0);
        if (row.has_value(1))
        {
            this->guardedness = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->Dugald = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->polymaths = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->thighbone = ((Type_Madison)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->astronomy = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->latish = row.to_uint32(6);
        }
        if (row.has_value(7))
        {
            this->Taney = row.to_boolean(7);
        }
        if (row.has_value(8))
        {
            this->Mable = row.to_int8(8);
        }
        if (row.has_value(9))
        {
            this->emblazon = row.to_datetime(9);
        }
        if (row.has_value(10))
        {
            this->monorail = row.to_uint64(10);
        }
        if (row.has_value(11))
        {
            this->cowhand = row.to_int16(11);
        }
        if (row.has_value(12))
        {
            this->fisticuff = row.to_boolean(12);
        }
        if (row.has_value(13))
        {
            this->germicidal = row.to_single(13);
        }
        if (row.has_value(14))
        {
            this->collinear = ((Type_Madison)(row.to_int32(14)));
        }
        if ((this->Child_dissociable == nullptr))
        {
            this->Child_dissociable = &(Table33Row::Child_dissociableEmpty);
        }
        this->SetKey(this->meatpacking);
    }
    
    void Table33SetChild_dissociable(Table33Row* target, const std::vector<Table33Child_dissociableRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_dissociable = new Table33Child_dissociableTable(childs);
    }
    
    Table33Table::Table33Table()
    {
    }
    
    Table33Table::Table33Table(CremaReader::itable& table)
    {
        this->Child_dissociable = new Table33Child_dissociableTable(table.dataset().tables()["Table33.Child_dissociable"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_dissociable->Rows, Table33SetChild_dissociable);
    }
    
    Table33Table::~Table33Table()
    {
        delete this->Child_dissociable;
    }
    
    void* Table33Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table33Row(row, ((Table33Table*)(table)));
    }
    
    const Table33Row* Table33Table::Find(double meatpacking) const
    {
        return this->FindRow(meatpacking);
    }
    
    Table_drawnChild_BeardRow::Table_drawnChild_BeardRow(CremaReader::irow& row, Table_drawnChild_BeardTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->yang = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->embedding = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->sectionalized = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->backstabbing = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->omnibus = row.to_uint16(4);
        }
        this->SetKey(this->yang);
    }
    
    Table_drawnChild_BeardTable::Table_drawnChild_BeardTable()
    {
    }
    
    Table_drawnChild_BeardTable::Table_drawnChild_BeardTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_drawnChild_BeardTable::Table_drawnChild_BeardTable(std::vector<Table_drawnChild_BeardRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_drawnChild_BeardTable::~Table_drawnChild_BeardTable()
    {
    }
    
    void* Table_drawnChild_BeardTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_drawnChild_BeardRow(row, ((Table_drawnChild_BeardTable*)(table)));
    }
    
    const Table_drawnChild_BeardRow* Table_drawnChild_BeardTable::Find(bool yang) const
    {
        return this->FindRow(yang);
    }
    
    Table_drawnChild_BeardTable Table_drawnRow::Child_BeardEmpty;
    
    Table_drawnRow::Table_drawnRow(CremaReader::irow& row, Table_drawnTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Hereford = row.to_uint16(0);
        if ((this->Child_Beard == nullptr))
        {
            this->Child_Beard = &(Table_drawnRow::Child_BeardEmpty);
        }
        this->SetKey(this->Hereford);
    }
    
    void Table_drawnSetChild_Beard(Table_drawnRow* target, const std::vector<Table_drawnChild_BeardRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Beard = new Table_drawnChild_BeardTable(childs);
    }
    
    Table_drawnTable::Table_drawnTable()
    {
    }
    
    Table_drawnTable::Table_drawnTable(CremaReader::itable& table)
    {
        this->Child_Beard = new Table_drawnChild_BeardTable(table.dataset().tables()["Table_drawn.Child_Beard"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Beard->Rows, Table_drawnSetChild_Beard);
    }
    
    Table_drawnTable::~Table_drawnTable()
    {
        delete this->Child_Beard;
    }
    
    void* Table_drawnTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_drawnRow(row, ((Table_drawnTable*)(table)));
    }
    
    const Table_drawnRow* Table_drawnTable::Find(unsigned short Hereford) const
    {
        return this->FindRow(Hereford);
    }
    
    Table_HallinanRow::Table_HallinanRow(CremaReader::irow& row, Table_HallinanTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->victimize = row.to_int8(0);
        if (row.has_value(1))
        {
            this->overlay = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Endymion = ((Type36)(row.to_int32(2)));
        }
        this->SetKey(this->victimize);
    }
    
    Table_HallinanTable::Table_HallinanTable()
    {
    }
    
    Table_HallinanTable::Table_HallinanTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_HallinanTable::~Table_HallinanTable()
    {
    }
    
    void* Table_HallinanTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_HallinanRow(row, ((Table_HallinanTable*)(table)));
    }
    
    const Table_HallinanRow* Table_HallinanTable::Find(char victimize) const
    {
        return this->FindRow(victimize);
    }
    
    Table175Row::Table175Row(CremaReader::irow& row, Table175Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Cork = ((Type6)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Terrill = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->salubrious = ((Type_Mather)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->copora = row.to_int16(3);
        }
        if (row.has_value(4))
        {
            this->baggy = row.to_int32(4);
        }
        this->SetKey(this->Cork);
    }
    
    Table175Table::Table175Table()
    {
    }
    
    Table175Table::Table175Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table175Table::~Table175Table()
    {
    }
    
    void* Table175Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table175Row(row, ((Table175Table*)(table)));
    }
    
    const Table175Row* Table175Table::Find(Type6 Cork) const
    {
        return this->FindRow(Cork);
    }
    
    Table85Row::Table85Row(CremaReader::irow& row, Table85Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Chang = row.to_duration(0);
        if (row.has_value(1))
        {
            this->scrapheap = row.to_datetime(1);
        }
        this->breaststroke = row.to_int8(2);
        if (row.has_value(3))
        {
            this->allotting = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->cordiality = row.to_string(4);
        }
        this->SetKey(this->Chang, this->breaststroke);
    }
    
    Table85Table::Table85Table()
    {
    }
    
    Table85Table::Table85Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table85Table::~Table85Table()
    {
    }
    
    void* Table85Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table85Row(row, ((Table85Table*)(table)));
    }
    
    const Table85Row* Table85Table::Find(int Chang, char breaststroke) const
    {
        return this->FindRow(Chang, breaststroke);
    }
    
    Table151Row::Table151Row(CremaReader::irow& row, Table151Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Crissy = row.to_duration(0);
        if (row.has_value(1))
        {
            this->Aguilar = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->admirable = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Batsheva = ((Type9)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->Maurizio = row.to_single(4);
        }
        this->SetKey(this->Crissy);
    }
    
    Table151Table::Table151Table()
    {
    }
    
    Table151Table::Table151Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table151Table::~Table151Table()
    {
    }
    
    void* Table151Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table151Row(row, ((Table151Table*)(table)));
    }
    
    const Table151Row* Table151Table::Find(int Crissy) const
    {
        return this->FindRow(Crissy);
    }
    
    Table_alienRow::Table_alienRow(CremaReader::irow& row, Table_alienTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->snapback = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->caseworker = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Adonis = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->borderer = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->rattling = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->South = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->wagging = row.to_uint32(6);
        }
        if (row.has_value(7))
        {
            this->deleterious = row.to_duration(7);
        }
        if (row.has_value(8))
        {
            this->Dalmatian = row.to_int64(8);
        }
        this->SetKey(this->snapback);
    }
    
    Table_alienTable::Table_alienTable()
    {
    }
    
    Table_alienTable::Table_alienTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_alienTable::~Table_alienTable()
    {
    }
    
    void* Table_alienTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_alienRow(row, ((Table_alienTable*)(table)));
    }
    
    const Table_alienRow* Table_alienTable::Find(unsigned int snapback) const
    {
        return this->FindRow(snapback);
    }
    
    Table_crystallizesChild2Row::Table_crystallizesChild2Row(CremaReader::irow& row, Table_crystallizesChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->gos = row.to_int8(0);
        this->SetKey(this->gos);
    }
    
    Table_crystallizesChild2Table::Table_crystallizesChild2Table()
    {
    }
    
    Table_crystallizesChild2Table::Table_crystallizesChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_crystallizesChild2Table::Table_crystallizesChild2Table(std::vector<Table_crystallizesChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_crystallizesChild2Table::~Table_crystallizesChild2Table()
    {
    }
    
    void* Table_crystallizesChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_crystallizesChild2Row(row, ((Table_crystallizesChild2Table*)(table)));
    }
    
    const Table_crystallizesChild2Row* Table_crystallizesChild2Table::Find(char gos) const
    {
        return this->FindRow(gos);
    }
    
    Table_crystallizesChild3Row::Table_crystallizesChild3Row(CremaReader::irow& row, Table_crystallizesChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lovably = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->rendering = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->Wallace = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->superstructure = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->appearer = row.to_int16(4);
        }
        this->kitchenware = row.to_string(5);
        if (row.has_value(6))
        {
            this->Kora = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->silence = row.to_duration(7);
        }
        this->SetKey(this->lovably, this->kitchenware);
    }
    
    Table_crystallizesChild3Table::Table_crystallizesChild3Table()
    {
    }
    
    Table_crystallizesChild3Table::Table_crystallizesChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_crystallizesChild3Table::Table_crystallizesChild3Table(std::vector<Table_crystallizesChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_crystallizesChild3Table::~Table_crystallizesChild3Table()
    {
    }
    
    void* Table_crystallizesChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_crystallizesChild3Row(row, ((Table_crystallizesChild3Table*)(table)));
    }
    
    const Table_crystallizesChild3Row* Table_crystallizesChild3Table::Find(time_t lovably, const std::string& kitchenware) const
    {
        return this->FindRow(lovably, kitchenware);
    }
    
    Table_crystallizesChild2Table Table_crystallizesRow::Child2Empty;
    
    Table_crystallizesChild3Table Table_crystallizesRow::Child3Empty;
    
    Table_crystallizesRow::Table_crystallizesRow(CremaReader::irow& row, Table_crystallizesTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->reprehensible = ((Type3)(row.to_int32(0)));
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_crystallizesRow::Child2Empty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_crystallizesRow::Child3Empty);
        }
        this->SetKey(this->reprehensible);
    }
    
    void Table_crystallizesSetChild2(Table_crystallizesRow* target, const std::vector<Table_crystallizesChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_crystallizesChild2Table(childs);
    }
    
    void Table_crystallizesSetChild3(Table_crystallizesRow* target, const std::vector<Table_crystallizesChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_crystallizesChild3Table(childs);
    }
    
    Table_crystallizesTable::Table_crystallizesTable()
    {
    }
    
    Table_crystallizesTable::Table_crystallizesTable(CremaReader::itable& table)
    {
        this->Child2 = new Table_crystallizesChild2Table(table.dataset().tables()["Table_crystallizes.Child2"]);
        this->Child3 = new Table_crystallizesChild3Table(table.dataset().tables()["Table_crystallizes.Child3"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child2->Rows, Table_crystallizesSetChild2);
        this->SetRelations(this->Child3->Rows, Table_crystallizesSetChild3);
    }
    
    Table_crystallizesTable::~Table_crystallizesTable()
    {
        delete this->Child2;
        delete this->Child3;
    }
    
    void* Table_crystallizesTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_crystallizesRow(row, ((Table_crystallizesTable*)(table)));
    }
    
    const Table_crystallizesRow* Table_crystallizesTable::Find(Type3 reprehensible) const
    {
        return this->FindRow(reprehensible);
    }
    
    Table112Row::Table112Row(CremaReader::irow& row, Table112Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Lehigh = ((Type11)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Attic = row.to_datetime(1);
        }
        this->malaria = row.to_duration(2);
        if (row.has_value(3))
        {
            this->Chiarra = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->Pound = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->prescient = row.to_datetime(5);
        }
        if (row.has_value(6))
        {
            this->infarction = row.to_int16(6);
        }
        this->SetKey(this->Lehigh, this->malaria);
    }
    
    Table112Table::Table112Table()
    {
    }
    
    Table112Table::Table112Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table112Table::~Table112Table()
    {
    }
    
    void* Table112Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table112Row(row, ((Table112Table*)(table)));
    }
    
    const Table112Row* Table112Table::Find(Type11 Lehigh, int malaria) const
    {
        return this->FindRow(Lehigh, malaria);
    }
    
    Table116Row::Table116Row(CremaReader::irow& row, Table116Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hybridization = row.to_int16(0);
        if (row.has_value(1))
        {
            this->Blackstone = ((Type_Madison)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->adulator = ((Type_HeraclitusDeletable)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->durational = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->advertisement = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->mercuric = ((Type_Mather)(row.to_int32(5)));
        }
        this->SetKey(this->hybridization);
    }
    
    Table116Table::Table116Table()
    {
    }
    
    Table116Table::Table116Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table116Table::~Table116Table()
    {
    }
    
    void* Table116Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table116Row(row, ((Table116Table*)(table)));
    }
    
    const Table116Row* Table116Table::Find(short hybridization) const
    {
        return this->FindRow(hybridization);
    }
    
    Table199Row::Table199Row(CremaReader::irow& row, Table199Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->craze = ((Type79)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->gluon = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->bagged = row.to_uint16(2);
        }
        this->infringer = row.to_datetime(3);
        this->Kristen = row.to_int64(4);
        if (row.has_value(5))
        {
            this->Wendy = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->monoxide = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->plier = row.to_duration(7);
        }
        this->SetKey(this->craze, this->infringer, this->Kristen);
    }
    
    Table199Table::Table199Table()
    {
    }
    
    Table199Table::Table199Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table199Table::~Table199Table()
    {
    }
    
    void* Table199Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table199Row(row, ((Table199Table*)(table)));
    }
    
    const Table199Row* Table199Table::Find(Type79 craze, time_t infringer, long long Kristen) const
    {
        return this->FindRow(craze, infringer, Kristen);
    }
    
    Table9Child1Row::Table9Child1Row(CremaReader::irow& row, Table9Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hardhat = row.to_int64(0);
        if (row.has_value(1))
        {
            this->adulterant = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->anticyclonic = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->swoop = row.to_int32(3);
        }
        this->Kamehameha = row.to_int32(4);
        if (row.has_value(5))
        {
            this->cuckoo = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->Pavel = row.to_single(6);
        }
        if (row.has_value(7))
        {
            this->yesteryear = row.to_uint64(7);
        }
        if (row.has_value(8))
        {
            this->Ono = row.to_duration(8);
        }
        if (row.has_value(9))
        {
            this->titian = row.to_int64(9);
        }
        this->SetKey(this->hardhat, this->Kamehameha);
    }
    
    Table9Child1Table::Table9Child1Table()
    {
    }
    
    Table9Child1Table::Table9Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table9Child1Table::Table9Child1Table(std::vector<Table9Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table9Child1Table::~Table9Child1Table()
    {
    }
    
    void* Table9Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table9Child1Row(row, ((Table9Child1Table*)(table)));
    }
    
    const Table9Child1Row* Table9Child1Table::Find(long long hardhat, int Kamehameha) const
    {
        return this->FindRow(hardhat, Kamehameha);
    }
    
    Table9Child_warringDeletableRow::Table9Child_warringDeletableRow(CremaReader::irow& row, Table9Child_warringDeletableTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->koala = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->renovate = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Norplant = row.to_int64(2);
        }
        this->Palisades = row.to_boolean(3);
        this->ranter = row.to_double(4);
        if (row.has_value(5))
        {
            this->Agamemnon = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->viscountess = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->Adelbert = row.to_uint16(7);
        }
        this->chemiluminescence = row.to_uint64(8);
        if (row.has_value(9))
        {
            this->Uri = ((Type_canted)(row.to_int32(9)));
        }
        if (row.has_value(10))
        {
            this->stalker = row.to_datetime(10);
        }
        this->SetKey(this->koala, this->Palisades, this->ranter, this->chemiluminescence);
    }
    
    Table9Child_warringDeletableTable::Table9Child_warringDeletableTable()
    {
    }
    
    Table9Child_warringDeletableTable::Table9Child_warringDeletableTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table9Child_warringDeletableTable::Table9Child_warringDeletableTable(std::vector<Table9Child_warringDeletableRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table9Child_warringDeletableTable::~Table9Child_warringDeletableTable()
    {
    }
    
    void* Table9Child_warringDeletableTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table9Child_warringDeletableRow(row, ((Table9Child_warringDeletableTable*)(table)));
    }
    
    const Table9Child_warringDeletableRow* Table9Child_warringDeletableTable::Find(unsigned char koala, bool Palisades, double ranter, unsigned long long chemiluminescence) const
    {
        return this->FindRow(koala, Palisades, ranter, chemiluminescence);
    }
    
    Table9Child1Table Table9Row::Child1Empty;
    
    Table9Child_warringDeletableTable Table9Row::Child_warringDeletableEmpty;
    
    Table9Row::Table9Row(CremaReader::irow& row, Table9Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->saturate = row.to_string(0);
        if (row.has_value(1))
        {
            this->Selma = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->Urbana = ((Type_canted)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->diarrhea = row.to_uint16(3);
        }
        if (row.has_value(4))
        {
            this->DH = ((Type_Madison)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->graphics = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->bade = row.to_uint64(6);
        }
        if (row.has_value(7))
        {
            this->modesty = row.to_int16(7);
        }
        if (row.has_value(8))
        {
            this->implant = row.to_duration(8);
        }
        if (row.has_value(9))
        {
            this->epicyclic = row.to_single(9);
        }
        if (row.has_value(10))
        {
            this->pear = row.to_uint64(10);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table9Row::Child1Empty);
        }
        if ((this->Child_warringDeletable == nullptr))
        {
            this->Child_warringDeletable = &(Table9Row::Child_warringDeletableEmpty);
        }
        this->SetKey(this->saturate);
    }
    
    void Table9SetChild1(Table9Row* target, const std::vector<Table9Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table9Child1Table(childs);
    }
    
    void Table9SetChild_warringDeletable(Table9Row* target, const std::vector<Table9Child_warringDeletableRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_warringDeletable = new Table9Child_warringDeletableTable(childs);
    }
    
    Table9Table::Table9Table()
    {
    }
    
    Table9Table::Table9Table(CremaReader::itable& table)
    {
        this->Child1 = new Table9Child1Table(table.dataset().tables()["Table9.Child1"]);
        this->Child_warringDeletable = new Table9Child_warringDeletableTable(table.dataset().tables()["Table9.Child_warringDeletable"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table9SetChild1);
        this->SetRelations(this->Child_warringDeletable->Rows, Table9SetChild_warringDeletable);
    }
    
    Table9Table::~Table9Table()
    {
        delete this->Child1;
        delete this->Child_warringDeletable;
    }
    
    void* Table9Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table9Row(row, ((Table9Table*)(table)));
    }
    
    const Table9Row* Table9Table::Find(const std::string& saturate) const
    {
        return this->FindRow(saturate);
    }
    
    Table_piquantnessRow::Table_piquantnessRow(CremaReader::irow& row, Table_piquantnessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->culpableness = row.to_single(0);
        if (row.has_value(1))
        {
            this->spare = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->ltd = ((Type_Page)(row.to_int32(2)));
        }
        this->SetKey(this->culpableness);
    }
    
    Table_piquantnessTable::Table_piquantnessTable()
    {
    }
    
    Table_piquantnessTable::Table_piquantnessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_piquantnessTable::~Table_piquantnessTable()
    {
    }
    
    void* Table_piquantnessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_piquantnessRow(row, ((Table_piquantnessTable*)(table)));
    }
    
    const Table_piquantnessRow* Table_piquantnessTable::Find(float culpableness) const
    {
        return this->FindRow(culpableness);
    }
    
    Table111Row::Table111Row(CremaReader::irow& row, Table111Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->bani = row.to_int16(0);
        if (row.has_value(1))
        {
            this->sinusitis = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->foul = row.to_int16(2);
        }
        if (row.has_value(3))
        {
            this->foll = ((Type_HeraclitusDeletable)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->operable = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->soreness = row.to_int16(5);
        }
        if (row.has_value(6))
        {
            this->IT = row.to_uint64(6);
        }
        this->SetKey(this->bani);
    }
    
    Table111Table::Table111Table()
    {
    }
    
    Table111Table::Table111Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table111Table::~Table111Table()
    {
    }
    
    void* Table111Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table111Row(row, ((Table111Table*)(table)));
    }
    
    const Table111Row* Table111Table::Find(short bani) const
    {
        return this->FindRow(bani);
    }
    
    Table28Row::Table28Row(CremaReader::irow& row, Table28Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->error = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->Clarance = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->Gerardo = ((Type_livingness)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Chilean = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->vibrionic = row.to_int16(4);
        }
        if (row.has_value(5))
        {
            this->windflower = row.to_int16(5);
        }
        this->SetKey(this->error);
    }
    
    Table28Table::Table28Table()
    {
    }
    
    Table28Table::Table28Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table28Table::~Table28Table()
    {
    }
    
    void* Table28Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table28Row(row, ((Table28Table*)(table)));
    }
    
    const Table28Row* Table28Table::Find(time_t error) const
    {
        return this->FindRow(error);
    }
    
    Table_surveyChild1Row::Table_surveyChild1Row(CremaReader::irow& row, Table_surveyChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->photojournalism = row.to_int16(0);
        if (row.has_value(1))
        {
            this->panel = row.to_int64(1);
        }
        if (row.has_value(2))
        {
            this->welsher = ((Type_supportedDeletable)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->rump = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->iambic = ((Type_rifled)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->bogey = row.to_uint32(5);
        }
        this->expensiveness = row.to_duration(6);
        if (row.has_value(7))
        {
            this->pervasiveness = row.to_datetime(7);
        }
        this->SetKey(this->photojournalism, this->expensiveness);
    }
    
    Table_surveyChild1Table::Table_surveyChild1Table()
    {
    }
    
    Table_surveyChild1Table::Table_surveyChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_surveyChild1Table::Table_surveyChild1Table(std::vector<Table_surveyChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_surveyChild1Table::~Table_surveyChild1Table()
    {
    }
    
    void* Table_surveyChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_surveyChild1Row(row, ((Table_surveyChild1Table*)(table)));
    }
    
    const Table_surveyChild1Row* Table_surveyChild1Table::Find(short photojournalism, int expensiveness) const
    {
        return this->FindRow(photojournalism, expensiveness);
    }
    
    Table_surveyChild1Table Table_surveyRow::Child1Empty;
    
    Table_surveyRow::Table_surveyRow(CremaReader::irow& row, Table_surveyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Tanny = row.to_double(0);
        if (row.has_value(1))
        {
            this->fagoting = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->aim = ((Type_Madison)(row.to_int32(2)));
        }
        this->brutality = row.to_string(3);
        if (row.has_value(4))
        {
            this->scantly = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->British = row.to_int32(5);
        }
        if (row.has_value(6))
        {
            this->broadcast = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->injurer = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->dedicative = row.to_int16(8);
        }
        if (row.has_value(9))
        {
            this->puzzle = row.to_int32(9);
        }
        if (row.has_value(10))
        {
            this->lies = row.to_uint8(10);
        }
        if (row.has_value(11))
        {
            this->Heall = row.to_uint8(11);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_surveyRow::Child1Empty);
        }
        this->SetKey(this->Tanny, this->brutality);
    }
    
    void Table_surveySetChild1(Table_surveyRow* target, const std::vector<Table_surveyChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_surveyChild1Table(childs);
    }
    
    Table_surveyTable::Table_surveyTable()
    {
    }
    
    Table_surveyTable::Table_surveyTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_surveyChild1Table(table.dataset().tables()["Table_survey.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_surveySetChild1);
    }
    
    Table_surveyTable::~Table_surveyTable()
    {
        delete this->Child1;
    }
    
    void* Table_surveyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_surveyRow(row, ((Table_surveyTable*)(table)));
    }
    
    const Table_surveyRow* Table_surveyTable::Find(double Tanny, const std::string& brutality) const
    {
        return this->FindRow(Tanny, brutality);
    }
    
    Table128Child1Row::Table128Child1Row(CremaReader::irow& row, Table128Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Qaddafi = row.to_duration(0);
        if (row.has_value(1))
        {
            this->Hebraism = ((Type_applejack)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->limitless = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->Carlyle = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->we = row.to_int32(4);
        }
        if (row.has_value(5))
        {
            this->equipartition = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->metallurgical = ((Type55)(row.to_int32(6)));
        }
        this->SetKey(this->Qaddafi);
    }
    
    Table128Child1Table::Table128Child1Table()
    {
    }
    
    Table128Child1Table::Table128Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table128Child1Table::Table128Child1Table(std::vector<Table128Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table128Child1Table::~Table128Child1Table()
    {
    }
    
    void* Table128Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table128Child1Row(row, ((Table128Child1Table*)(table)));
    }
    
    const Table128Child1Row* Table128Child1Table::Find(int Qaddafi) const
    {
        return this->FindRow(Qaddafi);
    }
    
    Table128Child1Table Table128Row::Child1Empty;
    
    Table128Row::Table128Row(CremaReader::irow& row, Table128Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Peron = row.to_int64(0);
        if (row.has_value(1))
        {
            this->banisher = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->millimeter = row.to_single(2);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table128Row::Child1Empty);
        }
        this->SetKey(this->Peron);
    }
    
    void Table128SetChild1(Table128Row* target, const std::vector<Table128Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table128Child1Table(childs);
    }
    
    Table128Table::Table128Table()
    {
    }
    
    Table128Table::Table128Table(CremaReader::itable& table)
    {
        this->Child1 = new Table128Child1Table(table.dataset().tables()["Table128.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table128SetChild1);
    }
    
    Table128Table::~Table128Table()
    {
        delete this->Child1;
    }
    
    void* Table128Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table128Row(row, ((Table128Table*)(table)));
    }
    
    const Table128Row* Table128Table::Find(long long Peron) const
    {
        return this->FindRow(Peron);
    }
    
    Table31Row::Table31Row(CremaReader::irow& row, Table31Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->etc = row.to_duration(0);
        if (row.has_value(1))
        {
            this->puers = row.to_int64(1);
        }
        if (row.has_value(2))
        {
            this->freewheeling = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->demonstrator = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->allay = row.to_uint64(4);
        }
        if (row.has_value(5))
        {
            this->dethrone = ((Type_pledge)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Ann = row.to_int16(6);
        }
        this->SetKey(this->etc);
    }
    
    Table31Table::Table31Table()
    {
    }
    
    Table31Table::Table31Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table31Table::~Table31Table()
    {
    }
    
    void* Table31Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table31Row(row, ((Table31Table*)(table)));
    }
    
    const Table31Row* Table31Table::Find(int etc) const
    {
        return this->FindRow(etc);
    }
    
    Table34Row::Table34Row(CremaReader::irow& row, Table34Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->appellative = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->corpulence = row.to_uint64(1);
        }
        this->SetKey(this->appellative);
    }
    
    Table34Table::Table34Table()
    {
    }
    
    Table34Table::Table34Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table34Table::~Table34Table()
    {
    }
    
    void* Table34Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table34Row(row, ((Table34Table*)(table)));
    }
    
    const Table34Row* Table34Table::Find(time_t appellative) const
    {
        return this->FindRow(appellative);
    }
    
    Table_stockpileRow::Table_stockpileRow(CremaReader::irow& row, Table_stockpileTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outmaneuver = row.to_int64(0);
        if (row.has_value(1))
        {
            this->Heddi = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->sidle = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->kWh = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->toothpick = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->overstate = ((Type13)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->schoolyard = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->Tuareg = row.to_int16(7);
        }
        if (row.has_value(8))
        {
            this->Sennett = row.to_double(8);
        }
        if (row.has_value(9))
        {
            this->deicer = row.to_double(9);
        }
        if (row.has_value(10))
        {
            this->prefect = row.to_int64(10);
        }
        this->SetKey(this->outmaneuver);
    }
    
    Table_stockpileTable::Table_stockpileTable()
    {
    }
    
    Table_stockpileTable::Table_stockpileTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_stockpileTable::~Table_stockpileTable()
    {
    }
    
    void* Table_stockpileTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_stockpileRow(row, ((Table_stockpileTable*)(table)));
    }
    
    const Table_stockpileRow* Table_stockpileTable::Find(long long outmaneuver) const
    {
        return this->FindRow(outmaneuver);
    }
    
    Table44Child1Row::Table44Child1Row(CremaReader::irow& row, Table44Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->normalized = row.to_uint16(0);
        this->SetKey(this->normalized);
    }
    
    Table44Child1Table::Table44Child1Table()
    {
    }
    
    Table44Child1Table::Table44Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table44Child1Table::Table44Child1Table(std::vector<Table44Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table44Child1Table::~Table44Child1Table()
    {
    }
    
    void* Table44Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table44Child1Row(row, ((Table44Child1Table*)(table)));
    }
    
    const Table44Child1Row* Table44Child1Table::Find(unsigned short normalized) const
    {
        return this->FindRow(normalized);
    }
    
    Table44Child1Table Table44Row::Child1Empty;
    
    Table44Row::Table44Row(CremaReader::irow& row, Table44Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->pylori = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->zigzag = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->Appolonia = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->sahib = row.to_int64(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table44Row::Child1Empty);
        }
        this->SetKey(this->pylori);
    }
    
    void Table44SetChild1(Table44Row* target, const std::vector<Table44Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table44Child1Table(childs);
    }
    
    Table44Table::Table44Table()
    {
    }
    
    Table44Table::Table44Table(CremaReader::itable& table)
    {
        this->Child1 = new Table44Child1Table(table.dataset().tables()["Table44.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table44SetChild1);
    }
    
    Table44Table::~Table44Table()
    {
        delete this->Child1;
    }
    
    void* Table44Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table44Row(row, ((Table44Table*)(table)));
    }
    
    const Table44Row* Table44Table::Find(unsigned char pylori) const
    {
        return this->FindRow(pylori);
    }
    
    Table67Row::Table67Row(CremaReader::irow& row, Table67Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->event = ((Type_Gretta)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->serendipitous = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->Pawtucket = row.to_duration(2);
        }
        this->cordial = row.to_string(3);
        if (row.has_value(4))
        {
            this->eugenics = ((Type52)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->Sher = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->nemeses = row.to_int16(6);
        }
        if (row.has_value(7))
        {
            this->collimated = ((Type85)(row.to_int32(7)));
        }
        this->SetKey(this->event, this->cordial);
    }
    
    Table67Table::Table67Table()
    {
    }
    
    Table67Table::Table67Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table67Table::~Table67Table()
    {
    }
    
    void* Table67Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table67Row(row, ((Table67Table*)(table)));
    }
    
    const Table67Row* Table67Table::Find(Type_Gretta event, const std::string& cordial) const
    {
        return this->FindRow(event, cordial);
    }
    
    Table_accidentRow::Table_accidentRow(CremaReader::irow& row, Table_accidentTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Portsmouth = row.to_int16(0);
        if (row.has_value(1))
        {
            this->bout = ((Type_rennet)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->aerosol = ((Type_HeraclitusDeletable)(row.to_int32(2)));
        }
        this->SetKey(this->Portsmouth);
    }
    
    Table_accidentTable::Table_accidentTable()
    {
    }
    
    Table_accidentTable::Table_accidentTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_accidentTable::~Table_accidentTable()
    {
    }
    
    void* Table_accidentTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_accidentRow(row, ((Table_accidentTable*)(table)));
    }
    
    const Table_accidentRow* Table_accidentTable::Find(short Portsmouth) const
    {
        return this->FindRow(Portsmouth);
    }
    
    Table_symbioticChild1Row::Table_symbioticChild1Row(CremaReader::irow& row, Table_symbioticChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Patti = row.to_duration(0);
        if (row.has_value(1))
        {
            this->glazed = row.to_double(1);
        }
        this->diffract = row.to_boolean(2);
        if (row.has_value(3))
        {
            this->calamitous = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->Cranmer = row.to_int32(4);
        }
        if (row.has_value(5))
        {
            this->Volvo = row.to_duration(5);
        }
        this->SetKey(this->Patti, this->diffract);
    }
    
    Table_symbioticChild1Table::Table_symbioticChild1Table()
    {
    }
    
    Table_symbioticChild1Table::Table_symbioticChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_symbioticChild1Table::Table_symbioticChild1Table(std::vector<Table_symbioticChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_symbioticChild1Table::~Table_symbioticChild1Table()
    {
    }
    
    void* Table_symbioticChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_symbioticChild1Row(row, ((Table_symbioticChild1Table*)(table)));
    }
    
    const Table_symbioticChild1Row* Table_symbioticChild1Table::Find(int Patti, bool diffract) const
    {
        return this->FindRow(Patti, diffract);
    }
    
    Table_symbioticChild1Table Table_symbioticRow::Child1Empty;
    
    Table_symbioticRow::Table_symbioticRow(CremaReader::irow& row, Table_symbioticTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->snapback = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->caseworker = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Adonis = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->borderer = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->trollish = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->lukewarm = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->South = row.to_uint8(6);
        }
        if (row.has_value(7))
        {
            this->wagging = row.to_uint32(7);
        }
        if (row.has_value(8))
        {
            this->mechanism = ((Type20)(row.to_int32(8)));
        }
        if (row.has_value(9))
        {
            this->bobsledded = row.to_duration(9);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_symbioticRow::Child1Empty);
        }
        this->SetKey(this->snapback);
    }
    
    void Table_symbioticSetChild1(Table_symbioticRow* target, const std::vector<Table_symbioticChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_symbioticChild1Table(childs);
    }
    
    Table_symbioticTable::Table_symbioticTable()
    {
    }
    
    Table_symbioticTable::Table_symbioticTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_symbioticChild1Table(table.dataset().tables()["Table_symbiotic.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_symbioticSetChild1);
    }
    
    Table_symbioticTable::~Table_symbioticTable()
    {
        delete this->Child1;
    }
    
    void* Table_symbioticTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_symbioticRow(row, ((Table_symbioticTable*)(table)));
    }
    
    const Table_symbioticRow* Table_symbioticTable::Find(unsigned int snapback) const
    {
        return this->FindRow(snapback);
    }
    
    Table_ThornburgRow::Table_ThornburgRow(CremaReader::irow& row, Table_ThornburgTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lumberyard = ((Type6)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Carl = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->firearm = row.to_boolean(2);
        }
        this->uncap = row.to_int8(3);
        if (row.has_value(4))
        {
            this->chairwoman = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->Bassett = ((Type8)(row.to_int32(5)));
        }
        this->SetKey(this->lumberyard, this->uncap);
    }
    
    Table_ThornburgTable::Table_ThornburgTable()
    {
    }
    
    Table_ThornburgTable::Table_ThornburgTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_ThornburgTable::~Table_ThornburgTable()
    {
    }
    
    void* Table_ThornburgTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_ThornburgRow(row, ((Table_ThornburgTable*)(table)));
    }
    
    const Table_ThornburgRow* Table_ThornburgTable::Find(Type6 lumberyard, char uncap) const
    {
        return this->FindRow(lumberyard, uncap);
    }
    
    Table_vixenishRow::Table_vixenishRow(CremaReader::irow& row, Table_vixenishTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->corpsman = row.to_double(0);
        if (row.has_value(1))
        {
            this->Araucanian = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Kaposi = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->hyperemia = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->pensiveness = row.to_single(4);
        }
        this->jetting = row.to_boolean(5);
        if (row.has_value(6))
        {
            this->babe = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->clears = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->codetermine = row.to_duration(8);
        }
        this->SetKey(this->corpsman, this->jetting);
    }
    
    Table_vixenishTable::Table_vixenishTable()
    {
    }
    
    Table_vixenishTable::Table_vixenishTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_vixenishTable::~Table_vixenishTable()
    {
    }
    
    void* Table_vixenishTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_vixenishRow(row, ((Table_vixenishTable*)(table)));
    }
    
    const Table_vixenishRow* Table_vixenishTable::Find(double corpsman, bool jetting) const
    {
        return this->FindRow(corpsman, jetting);
    }
    
    Table142Row::Table142Row(CremaReader::irow& row, Table142Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lagging = row.to_single(0);
        if (row.has_value(1))
        {
            this->Ethyl = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->inflate = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->continuum = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->Kathryne = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->LCM = row.to_datetime(5);
        }
        if (row.has_value(6))
        {
            this->Marietta = row.to_uint16(6);
        }
        if (row.has_value(7))
        {
            this->petitioner = row.to_int32(7);
        }
        if (row.has_value(8))
        {
            this->Magdalena = row.to_duration(8);
        }
        if (row.has_value(9))
        {
            this->apprehend = row.to_uint16(9);
        }
        this->SetKey(this->lagging);
    }
    
    Table142Table::Table142Table()
    {
    }
    
    Table142Table::Table142Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table142Table::~Table142Table()
    {
    }
    
    void* Table142Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table142Row(row, ((Table142Table*)(table)));
    }
    
    const Table142Row* Table142Table::Find(float lagging) const
    {
        return this->FindRow(lagging);
    }
    
    Table96Row::Table96Row(CremaReader::irow& row, Table96Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->impoliticness = ((Type_gustily)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Roberto = row.to_int32(1);
        }
        this->hurried = ((Type_artiness)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->resplendent = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->papillae = row.to_int32(4);
        }
        this->popularism = row.to_int16(5);
        this->SetKey(this->impoliticness, this->hurried, this->popularism);
    }
    
    Table96Table::Table96Table()
    {
    }
    
    Table96Table::Table96Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table96Table::~Table96Table()
    {
    }
    
    void* Table96Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table96Row(row, ((Table96Table*)(table)));
    }
    
    const Table96Row* Table96Table::Find(Type_gustily impoliticness, Type_artiness hurried, short popularism) const
    {
        return this->FindRow(impoliticness, hurried, popularism);
    }
    
    Table98Child1Row::Table98Child1Row(CremaReader::irow& row, Table98Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->syncopation = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Rayner = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->abjure = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->Nukualofa = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->escrow = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->thriver = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->gigged = ((Type_Porrima)(row.to_int32(6)));
        }
        this->SetKey(this->syncopation);
    }
    
    Table98Child1Table::Table98Child1Table()
    {
    }
    
    Table98Child1Table::Table98Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table98Child1Table::Table98Child1Table(std::vector<Table98Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table98Child1Table::~Table98Child1Table()
    {
    }
    
    void* Table98Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table98Child1Row(row, ((Table98Child1Table*)(table)));
    }
    
    const Table98Child1Row* Table98Child1Table::Find(int syncopation) const
    {
        return this->FindRow(syncopation);
    }
    
    Table98Child1Table Table98Row::Child1Empty;
    
    Table98Row::Table98Row(CremaReader::irow& row, Table98Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->distension = row.to_int32(0);
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table98Row::Child1Empty);
        }
        this->SetKey(this->distension);
    }
    
    void Table98SetChild1(Table98Row* target, const std::vector<Table98Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table98Child1Table(childs);
    }
    
    Table98Table::Table98Table()
    {
    }
    
    Table98Table::Table98Table(CremaReader::itable& table)
    {
        this->Child1 = new Table98Child1Table(table.dataset().tables()["Table98.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table98SetChild1);
    }
    
    Table98Table::~Table98Table()
    {
        delete this->Child1;
    }
    
    void* Table98Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table98Row(row, ((Table98Table*)(table)));
    }
    
    const Table98Row* Table98Table::Find(int distension) const
    {
        return this->FindRow(distension);
    }
    
    Table_refunderChild1Row::Table_refunderChild1Row(CremaReader::irow& row, Table_refunderChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hauberk = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->tortoiseshell = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->costarring = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->cod = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->SEATO = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->shucker = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->gasworks = row.to_datetime(6);
        }
        this->SetKey(this->hauberk);
    }
    
    Table_refunderChild1Table::Table_refunderChild1Table()
    {
    }
    
    Table_refunderChild1Table::Table_refunderChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_refunderChild1Table::Table_refunderChild1Table(std::vector<Table_refunderChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_refunderChild1Table::~Table_refunderChild1Table()
    {
    }
    
    void* Table_refunderChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_refunderChild1Row(row, ((Table_refunderChild1Table*)(table)));
    }
    
    const Table_refunderChild1Row* Table_refunderChild1Table::Find(unsigned char hauberk) const
    {
        return this->FindRow(hauberk);
    }
    
    Table_refunderChild1Table Table_refunderRow::Child1Empty;
    
    Table_refunderRow::Table_refunderRow(CremaReader::irow& row, Table_refunderTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->meatpacking = row.to_double(0);
        if (row.has_value(1))
        {
            this->guardedness = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->Dugald = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->polymaths = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->thighbone = ((Type_Madison)(row.to_int32(4)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_refunderRow::Child1Empty);
        }
        this->SetKey(this->meatpacking);
    }
    
    void Table_refunderSetChild1(Table_refunderRow* target, const std::vector<Table_refunderChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_refunderChild1Table(childs);
    }
    
    Table_refunderTable::Table_refunderTable()
    {
    }
    
    Table_refunderTable::Table_refunderTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_refunderChild1Table(table.dataset().tables()["Table_refunder.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_refunderSetChild1);
    }
    
    Table_refunderTable::~Table_refunderTable()
    {
        delete this->Child1;
    }
    
    void* Table_refunderTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_refunderRow(row, ((Table_refunderTable*)(table)));
    }
    
    const Table_refunderRow* Table_refunderTable::Find(double meatpacking) const
    {
        return this->FindRow(meatpacking);
    }
    
    Table_wolvesChild1Row::Table_wolvesChild1Row(CremaReader::irow& row, Table_wolvesChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Jolee = row.to_int8(0);
        if (row.has_value(1))
        {
            this->singletree = ((Type_Mather)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->blunderer = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->condolence = ((Type70)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->runaway = row.to_duration(4);
        }
        this->precious = row.to_single(5);
        if (row.has_value(6))
        {
            this->Anstice = row.to_int32(6);
        }
        if (row.has_value(7))
        {
            this->shielder = row.to_uint16(7);
        }
        this->SetKey(this->Jolee, this->precious);
    }
    
    Table_wolvesChild1Table::Table_wolvesChild1Table()
    {
    }
    
    Table_wolvesChild1Table::Table_wolvesChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_wolvesChild1Table::Table_wolvesChild1Table(std::vector<Table_wolvesChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_wolvesChild1Table::~Table_wolvesChild1Table()
    {
    }
    
    void* Table_wolvesChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_wolvesChild1Row(row, ((Table_wolvesChild1Table*)(table)));
    }
    
    const Table_wolvesChild1Row* Table_wolvesChild1Table::Find(char Jolee, float precious) const
    {
        return this->FindRow(Jolee, precious);
    }
    
    Table_wolvesChild1Table Table_wolvesRow::Child1Empty;
    
    Table_wolvesRow::Table_wolvesRow(CremaReader::irow& row, Table_wolvesTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->tempura = row.to_uint32(0);
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_wolvesRow::Child1Empty);
        }
        this->SetKey(this->tempura);
    }
    
    void Table_wolvesSetChild1(Table_wolvesRow* target, const std::vector<Table_wolvesChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_wolvesChild1Table(childs);
    }
    
    Table_wolvesTable::Table_wolvesTable()
    {
    }
    
    Table_wolvesTable::Table_wolvesTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_wolvesChild1Table(table.dataset().tables()["Table_wolves.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_wolvesSetChild1);
    }
    
    Table_wolvesTable::~Table_wolvesTable()
    {
        delete this->Child1;
    }
    
    void* Table_wolvesTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_wolvesRow(row, ((Table_wolvesTable*)(table)));
    }
    
    const Table_wolvesRow* Table_wolvesTable::Find(unsigned int tempura) const
    {
        return this->FindRow(tempura);
    }
    
    Table45Child1Row::Table45Child1Row(CremaReader::irow& row, Table45Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->intractable = row.to_int32(0);
        this->romanticist = row.to_duration(1);
        this->SetKey(this->intractable, this->romanticist);
    }
    
    Table45Child1Table::Table45Child1Table()
    {
    }
    
    Table45Child1Table::Table45Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table45Child1Table::Table45Child1Table(std::vector<Table45Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table45Child1Table::~Table45Child1Table()
    {
    }
    
    void* Table45Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table45Child1Row(row, ((Table45Child1Table*)(table)));
    }
    
    const Table45Child1Row* Table45Child1Table::Find(int intractable, int romanticist) const
    {
        return this->FindRow(intractable, romanticist);
    }
    
    Table45Child1Table Table45Row::Child1Empty;
    
    Table45Row::Table45Row(CremaReader::irow& row, Table45Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->monumental = row.to_int32(0);
        if (row.has_value(1))
        {
            this->nonplussed = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->pterodactyl = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->concerted = row.to_int16(3);
        }
        if (row.has_value(4))
        {
            this->refrozen = row.to_int8(4);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table45Row::Child1Empty);
        }
        this->SetKey(this->monumental);
    }
    
    void Table45SetChild1(Table45Row* target, const std::vector<Table45Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table45Child1Table(childs);
    }
    
    Table45Table::Table45Table()
    {
    }
    
    Table45Table::Table45Table(CremaReader::itable& table)
    {
        this->Child1 = new Table45Child1Table(table.dataset().tables()["Table45.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table45SetChild1);
    }
    
    Table45Table::~Table45Table()
    {
        delete this->Child1;
    }
    
    void* Table45Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table45Row(row, ((Table45Table*)(table)));
    }
    
    const Table45Row* Table45Table::Find(int monumental) const
    {
        return this->FindRow(monumental);
    }
    
    Table83Child1Row::Table83Child1Row(CremaReader::irow& row, Table83Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->feign = row.to_int64(0);
        if (row.has_value(1))
        {
            this->snob = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->lobby = row.to_double(2);
        }
        this->SetKey(this->feign);
    }
    
    Table83Child1Table::Table83Child1Table()
    {
    }
    
    Table83Child1Table::Table83Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table83Child1Table::Table83Child1Table(std::vector<Table83Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table83Child1Table::~Table83Child1Table()
    {
    }
    
    void* Table83Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table83Child1Row(row, ((Table83Child1Table*)(table)));
    }
    
    const Table83Child1Row* Table83Child1Table::Find(long long feign) const
    {
        return this->FindRow(feign);
    }
    
    Table83Child1Table Table83Row::Child1Empty;
    
    Table83Row::Table83Row(CremaReader::irow& row, Table83Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->offstage = row.to_int64(0);
        if (row.has_value(1))
        {
            this->pinhead = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->fancily = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->starlet = row.to_int64(3);
        }
        this->perkiness = row.to_string(4);
        if (row.has_value(5))
        {
            this->genially = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->flax = ((Type_gustily)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->brickbat = row.to_uint64(7);
        }
        if (row.has_value(8))
        {
            this->Clemente = ((Type_surprise)(row.to_int32(8)));
        }
        if (row.has_value(9))
        {
            this->fractious = ((Type_Dianna)(row.to_int32(9)));
        }
        if (row.has_value(10))
        {
            this->unsightliness = row.to_boolean(10);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table83Row::Child1Empty);
        }
        this->SetKey(this->offstage, this->perkiness);
    }
    
    void Table83SetChild1(Table83Row* target, const std::vector<Table83Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table83Child1Table(childs);
    }
    
    Table83Table::Table83Table()
    {
    }
    
    Table83Table::Table83Table(CremaReader::itable& table)
    {
        this->Child1 = new Table83Child1Table(table.dataset().tables()["Table83.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table83SetChild1);
    }
    
    Table83Table::~Table83Table()
    {
        delete this->Child1;
    }
    
    void* Table83Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table83Row(row, ((Table83Table*)(table)));
    }
    
    const Table83Row* Table83Table::Find(long long offstage, const std::string& perkiness) const
    {
        return this->FindRow(offstage, perkiness);
    }
    
    Table_sherbetChild_GallupRow::Table_sherbetChild_GallupRow(CremaReader::irow& row, Table_sherbetChild_GallupTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hardhat = row.to_int64(0);
        if (row.has_value(1))
        {
            this->capacity = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->adulterant = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->anticyclonic = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->swoop = row.to_int32(4);
        }
        this->Kamehameha = row.to_int32(5);
        if (row.has_value(6))
        {
            this->cuckoo = row.to_double(6);
        }
        this->SetKey(this->hardhat, this->Kamehameha);
    }
    
    Table_sherbetChild_GallupTable::Table_sherbetChild_GallupTable()
    {
    }
    
    Table_sherbetChild_GallupTable::Table_sherbetChild_GallupTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_sherbetChild_GallupTable::Table_sherbetChild_GallupTable(std::vector<Table_sherbetChild_GallupRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_sherbetChild_GallupTable::~Table_sherbetChild_GallupTable()
    {
    }
    
    void* Table_sherbetChild_GallupTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_sherbetChild_GallupRow(row, ((Table_sherbetChild_GallupTable*)(table)));
    }
    
    const Table_sherbetChild_GallupRow* Table_sherbetChild_GallupTable::Find(long long hardhat, int Kamehameha) const
    {
        return this->FindRow(hardhat, Kamehameha);
    }
    
    Table_sherbetChild3Row::Table_sherbetChild3Row(CremaReader::irow& row, Table_sherbetChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->koala = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->renovate = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Norplant = row.to_int64(2);
        }
        this->Palisades = row.to_boolean(3);
        this->ranter = row.to_double(4);
        if (row.has_value(5))
        {
            this->Agamemnon = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->viscountess = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->Adelbert = row.to_uint16(7);
        }
        this->chemiluminescence = row.to_uint64(8);
        this->SetKey(this->koala, this->Palisades, this->ranter, this->chemiluminescence);
    }
    
    Table_sherbetChild3Table::Table_sherbetChild3Table()
    {
    }
    
    Table_sherbetChild3Table::Table_sherbetChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_sherbetChild3Table::Table_sherbetChild3Table(std::vector<Table_sherbetChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_sherbetChild3Table::~Table_sherbetChild3Table()
    {
    }
    
    void* Table_sherbetChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_sherbetChild3Row(row, ((Table_sherbetChild3Table*)(table)));
    }
    
    const Table_sherbetChild3Row* Table_sherbetChild3Table::Find(unsigned char koala, bool Palisades, double ranter, unsigned long long chemiluminescence) const
    {
        return this->FindRow(koala, Palisades, ranter, chemiluminescence);
    }
    
    Table_sherbetChild_GallupTable Table_sherbetRow::Child_GallupEmpty;
    
    Table_sherbetChild3Table Table_sherbetRow::Child3Empty;
    
    Table_sherbetRow::Table_sherbetRow(CremaReader::irow& row, Table_sherbetTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->saturate = row.to_string(0);
        if (row.has_value(1))
        {
            this->Selma = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->Urbana = ((Type_canted)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->diarrhea = row.to_uint16(3);
        }
        if (row.has_value(4))
        {
            this->DH = ((Type_Madison)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->torque = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->submissiveness = row.to_single(6);
        }
        if ((this->Child_Gallup == nullptr))
        {
            this->Child_Gallup = &(Table_sherbetRow::Child_GallupEmpty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_sherbetRow::Child3Empty);
        }
        this->SetKey(this->saturate);
    }
    
    void Table_sherbetSetChild_Gallup(Table_sherbetRow* target, const std::vector<Table_sherbetChild_GallupRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Gallup = new Table_sherbetChild_GallupTable(childs);
    }
    
    void Table_sherbetSetChild3(Table_sherbetRow* target, const std::vector<Table_sherbetChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_sherbetChild3Table(childs);
    }
    
    Table_sherbetTable::Table_sherbetTable()
    {
    }
    
    Table_sherbetTable::Table_sherbetTable(CremaReader::itable& table)
    {
        this->Child_Gallup = new Table_sherbetChild_GallupTable(table.dataset().tables()["Table_sherbet.Child_Gallup"]);
        this->Child3 = new Table_sherbetChild3Table(table.dataset().tables()["Table_sherbet.Child3"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Gallup->Rows, Table_sherbetSetChild_Gallup);
        this->SetRelations(this->Child3->Rows, Table_sherbetSetChild3);
    }
    
    Table_sherbetTable::~Table_sherbetTable()
    {
        delete this->Child_Gallup;
        delete this->Child3;
    }
    
    void* Table_sherbetTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_sherbetRow(row, ((Table_sherbetTable*)(table)));
    }
    
    const Table_sherbetRow* Table_sherbetTable::Find(const std::string& saturate) const
    {
        return this->FindRow(saturate);
    }
    
    Table_freighterRow::Table_freighterRow(CremaReader::irow& row, Table_freighterTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Argentina = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->wingding = ((Type_Multan)(row.to_int32(1)));
        }
        this->SetKey(this->Argentina);
    }
    
    Table_freighterTable::Table_freighterTable()
    {
    }
    
    Table_freighterTable::Table_freighterTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_freighterTable::~Table_freighterTable()
    {
    }
    
    void* Table_freighterTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_freighterRow(row, ((Table_freighterTable*)(table)));
    }
    
    const Table_freighterRow* Table_freighterTable::Find(unsigned int Argentina) const
    {
        return this->FindRow(Argentina);
    }
    
    Table_HanoverianChild_closingRow::Table_HanoverianChild_closingRow(CremaReader::irow& row, Table_HanoverianChild_closingTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Shackleton = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->austereness = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->casaba = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->reflection = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->publishes = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->nonempty = row.to_int16(5);
        }
        this->SetKey(this->Shackleton);
    }
    
    Table_HanoverianChild_closingTable::Table_HanoverianChild_closingTable()
    {
    }
    
    Table_HanoverianChild_closingTable::Table_HanoverianChild_closingTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_HanoverianChild_closingTable::Table_HanoverianChild_closingTable(std::vector<Table_HanoverianChild_closingRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_HanoverianChild_closingTable::~Table_HanoverianChild_closingTable()
    {
    }
    
    void* Table_HanoverianChild_closingTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_HanoverianChild_closingRow(row, ((Table_HanoverianChild_closingTable*)(table)));
    }
    
    const Table_HanoverianChild_closingRow* Table_HanoverianChild_closingTable::Find(unsigned long long Shackleton) const
    {
        return this->FindRow(Shackleton);
    }
    
    Table_HanoverianChild_MartinoRow::Table_HanoverianChild_MartinoRow(CremaReader::irow& row, Table_HanoverianChild_MartinoTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->whitening = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->anthropometric = row.to_int32(1);
        }
        this->bartend = row.to_int32(2);
        this->SetKey(this->whitening, this->bartend);
    }
    
    Table_HanoverianChild_MartinoTable::Table_HanoverianChild_MartinoTable()
    {
    }
    
    Table_HanoverianChild_MartinoTable::Table_HanoverianChild_MartinoTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_HanoverianChild_MartinoTable::Table_HanoverianChild_MartinoTable(std::vector<Table_HanoverianChild_MartinoRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_HanoverianChild_MartinoTable::~Table_HanoverianChild_MartinoTable()
    {
    }
    
    void* Table_HanoverianChild_MartinoTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_HanoverianChild_MartinoRow(row, ((Table_HanoverianChild_MartinoTable*)(table)));
    }
    
    const Table_HanoverianChild_MartinoRow* Table_HanoverianChild_MartinoTable::Find(time_t whitening, int bartend) const
    {
        return this->FindRow(whitening, bartend);
    }
    
    Table_HanoverianChild_closingTable Table_HanoverianRow::Child_closingEmpty;
    
    Table_HanoverianChild_MartinoTable Table_HanoverianRow::Child_MartinoEmpty;
    
    Table_HanoverianRow::Table_HanoverianRow(CremaReader::irow& row, Table_HanoverianTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Stendhal = row.to_datetime(0);
        this->cheerer = row.to_int32(1);
        if (row.has_value(2))
        {
            this->wreckage = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Andriette = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->oviduct = row.to_int64(4);
        }
        if ((this->Child_closing == nullptr))
        {
            this->Child_closing = &(Table_HanoverianRow::Child_closingEmpty);
        }
        if ((this->Child_Martino == nullptr))
        {
            this->Child_Martino = &(Table_HanoverianRow::Child_MartinoEmpty);
        }
        this->SetKey(this->Stendhal, this->cheerer);
    }
    
    void Table_HanoverianSetChild_closing(Table_HanoverianRow* target, const std::vector<Table_HanoverianChild_closingRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_closing = new Table_HanoverianChild_closingTable(childs);
    }
    
    void Table_HanoverianSetChild_Martino(Table_HanoverianRow* target, const std::vector<Table_HanoverianChild_MartinoRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Martino = new Table_HanoverianChild_MartinoTable(childs);
    }
    
    Table_HanoverianTable::Table_HanoverianTable()
    {
    }
    
    Table_HanoverianTable::Table_HanoverianTable(CremaReader::itable& table)
    {
        this->Child_closing = new Table_HanoverianChild_closingTable(table.dataset().tables()["Table_Hanoverian.Child_closing"]);
        this->Child_Martino = new Table_HanoverianChild_MartinoTable(table.dataset().tables()["Table_Hanoverian.Child_Martino"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_closing->Rows, Table_HanoverianSetChild_closing);
        this->SetRelations(this->Child_Martino->Rows, Table_HanoverianSetChild_Martino);
    }
    
    Table_HanoverianTable::~Table_HanoverianTable()
    {
        delete this->Child_closing;
        delete this->Child_Martino;
    }
    
    void* Table_HanoverianTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_HanoverianRow(row, ((Table_HanoverianTable*)(table)));
    }
    
    const Table_HanoverianRow* Table_HanoverianTable::Find(time_t Stendhal, int cheerer) const
    {
        return this->FindRow(Stendhal, cheerer);
    }
    
    Table_houriChild1Row::Table_houriChild1Row(CremaReader::irow& row, Table_houriChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hauberk = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->tortoiseshell = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->costarring = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->cod = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->SEATO = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->shucker = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->gasworks = row.to_datetime(6);
        }
        this->SetKey(this->hauberk);
    }
    
    Table_houriChild1Table::Table_houriChild1Table()
    {
    }
    
    Table_houriChild1Table::Table_houriChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_houriChild1Table::Table_houriChild1Table(std::vector<Table_houriChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_houriChild1Table::~Table_houriChild1Table()
    {
    }
    
    void* Table_houriChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_houriChild1Row(row, ((Table_houriChild1Table*)(table)));
    }
    
    const Table_houriChild1Row* Table_houriChild1Table::Find(unsigned char hauberk) const
    {
        return this->FindRow(hauberk);
    }
    
    Table_houriChild1Table Table_houriRow::Child1Empty;
    
    Table_houriRow::Table_houriRow(CremaReader::irow& row, Table_houriTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->meatpacking = row.to_double(0);
        if (row.has_value(1))
        {
            this->guardedness = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->Dugald = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->polymaths = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->thighbone = ((Type_Madison)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->renovate = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->visible = row.to_uint64(6);
        }
        if (row.has_value(7))
        {
            this->typeahead = row.to_duration(7);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_houriRow::Child1Empty);
        }
        this->SetKey(this->meatpacking);
    }
    
    void Table_houriSetChild1(Table_houriRow* target, const std::vector<Table_houriChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_houriChild1Table(childs);
    }
    
    Table_houriTable::Table_houriTable()
    {
    }
    
    Table_houriTable::Table_houriTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_houriChild1Table(table.dataset().tables()["Table_houri.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_houriSetChild1);
    }
    
    Table_houriTable::~Table_houriTable()
    {
        delete this->Child1;
    }
    
    void* Table_houriTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_houriRow(row, ((Table_houriTable*)(table)));
    }
    
    const Table_houriRow* Table_houriTable::Find(double meatpacking) const
    {
        return this->FindRow(meatpacking);
    }
    
    Table191Row::Table191Row(CremaReader::irow& row, Table191Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->emergence = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->plebiscite = ((Type77)(row.to_int32(1)));
        }
        this->christened = ((Type_rennet)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->cylindric = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->cryostat = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->cotillion = ((Type26)(row.to_int32(5)));
        }
        this->SetKey(this->emergence, this->christened);
    }
    
    Table191Table::Table191Table()
    {
    }
    
    Table191Table::Table191Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table191Table::~Table191Table()
    {
    }
    
    void* Table191Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table191Row(row, ((Table191Table*)(table)));
    }
    
    const Table191Row* Table191Table::Find(bool emergence, Type_rennet christened) const
    {
        return this->FindRow(emergence, christened);
    }
    
    Table_lunchtimeRow::Table_lunchtimeRow(CremaReader::irow& row, Table_lunchtimeTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Chang = row.to_duration(0);
        if (row.has_value(1))
        {
            this->scrapheap = row.to_datetime(1);
        }
        this->breaststroke = row.to_int8(2);
        if (row.has_value(3))
        {
            this->allotting = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->cordiality = row.to_string(4);
        }
        this->SetKey(this->Chang, this->breaststroke);
    }
    
    Table_lunchtimeTable::Table_lunchtimeTable()
    {
    }
    
    Table_lunchtimeTable::Table_lunchtimeTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_lunchtimeTable::~Table_lunchtimeTable()
    {
    }
    
    void* Table_lunchtimeTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_lunchtimeRow(row, ((Table_lunchtimeTable*)(table)));
    }
    
    const Table_lunchtimeRow* Table_lunchtimeTable::Find(int Chang, char breaststroke) const
    {
        return this->FindRow(Chang, breaststroke);
    }
    
    Table165Row::Table165Row(CremaReader::irow& row, Table165Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->impersonality = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->intercession = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->meiosis = ((Type_Page)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->nut = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->psychedelic = row.to_int16(4);
        }
        this->SetKey(this->impersonality);
    }
    
    Table165Table::Table165Table()
    {
    }
    
    Table165Table::Table165Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table165Table::~Table165Table()
    {
    }
    
    void* Table165Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table165Row(row, ((Table165Table*)(table)));
    }
    
    const Table165Row* Table165Table::Find(bool impersonality) const
    {
        return this->FindRow(impersonality);
    }
    
    Table_dreamlessChild1Row::Table_dreamlessChild1Row(CremaReader::irow& row, Table_dreamlessChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->pill = row.to_boolean(3);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table_dreamlessChild1Table::Table_dreamlessChild1Table()
    {
    }
    
    Table_dreamlessChild1Table::Table_dreamlessChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_dreamlessChild1Table::Table_dreamlessChild1Table(std::vector<Table_dreamlessChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_dreamlessChild1Table::~Table_dreamlessChild1Table()
    {
    }
    
    void* Table_dreamlessChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_dreamlessChild1Row(row, ((Table_dreamlessChild1Table*)(table)));
    }
    
    const Table_dreamlessChild1Row* Table_dreamlessChild1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table_dreamlessChild_newsprintRow::Table_dreamlessChild_newsprintRow(CremaReader::irow& row, Table_dreamlessChild_newsprintTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->condominium = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->guiltlessness = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->splash = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->entrapping = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->Hamnet = row.to_datetime(5);
        }
        this->SetKey(this->outgrip);
    }
    
    Table_dreamlessChild_newsprintTable::Table_dreamlessChild_newsprintTable()
    {
    }
    
    Table_dreamlessChild_newsprintTable::Table_dreamlessChild_newsprintTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_dreamlessChild_newsprintTable::Table_dreamlessChild_newsprintTable(std::vector<Table_dreamlessChild_newsprintRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_dreamlessChild_newsprintTable::~Table_dreamlessChild_newsprintTable()
    {
    }
    
    void* Table_dreamlessChild_newsprintTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_dreamlessChild_newsprintRow(row, ((Table_dreamlessChild_newsprintTable*)(table)));
    }
    
    const Table_dreamlessChild_newsprintRow* Table_dreamlessChild_newsprintTable::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table_dreamlessChild_nevusRow::Table_dreamlessChild_nevusRow(CremaReader::irow& row, Table_dreamlessChild_nevusTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->repetition = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->phonetician = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->Nanni = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->monographs = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->humus = row.to_int64(4);
        }
        this->SetKey(this->repetition);
    }
    
    Table_dreamlessChild_nevusTable::Table_dreamlessChild_nevusTable()
    {
    }
    
    Table_dreamlessChild_nevusTable::Table_dreamlessChild_nevusTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_dreamlessChild_nevusTable::Table_dreamlessChild_nevusTable(std::vector<Table_dreamlessChild_nevusRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_dreamlessChild_nevusTable::~Table_dreamlessChild_nevusTable()
    {
    }
    
    void* Table_dreamlessChild_nevusTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_dreamlessChild_nevusRow(row, ((Table_dreamlessChild_nevusTable*)(table)));
    }
    
    const Table_dreamlessChild_nevusRow* Table_dreamlessChild_nevusTable::Find(unsigned char repetition) const
    {
        return this->FindRow(repetition);
    }
    
    Table_dreamlessChild3Row::Table_dreamlessChild3Row(CremaReader::irow& row, Table_dreamlessChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->vaginae = row.to_int64(0);
        if (row.has_value(1))
        {
            this->obvious = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->sachet = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->underpayment = ((Type11)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->axle = row.to_int8(4);
        }
        this->SetKey(this->vaginae);
    }
    
    Table_dreamlessChild3Table::Table_dreamlessChild3Table()
    {
    }
    
    Table_dreamlessChild3Table::Table_dreamlessChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_dreamlessChild3Table::Table_dreamlessChild3Table(std::vector<Table_dreamlessChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_dreamlessChild3Table::~Table_dreamlessChild3Table()
    {
    }
    
    void* Table_dreamlessChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_dreamlessChild3Row(row, ((Table_dreamlessChild3Table*)(table)));
    }
    
    const Table_dreamlessChild3Row* Table_dreamlessChild3Table::Find(long long vaginae) const
    {
        return this->FindRow(vaginae);
    }
    
    Table_dreamlessChild2Row::Table_dreamlessChild2Row(CremaReader::irow& row, Table_dreamlessChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Enos = row.to_single(0);
        if (row.has_value(1))
        {
            this->pervasive = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->lubricator = row.to_boolean(2);
        }
        this->hallmark = row.to_string(3);
        this->SetKey(this->Enos, this->hallmark);
    }
    
    Table_dreamlessChild2Table::Table_dreamlessChild2Table()
    {
    }
    
    Table_dreamlessChild2Table::Table_dreamlessChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_dreamlessChild2Table::Table_dreamlessChild2Table(std::vector<Table_dreamlessChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_dreamlessChild2Table::~Table_dreamlessChild2Table()
    {
    }
    
    void* Table_dreamlessChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_dreamlessChild2Row(row, ((Table_dreamlessChild2Table*)(table)));
    }
    
    const Table_dreamlessChild2Row* Table_dreamlessChild2Table::Find(float Enos, const std::string& hallmark) const
    {
        return this->FindRow(Enos, hallmark);
    }
    
    Table_dreamlessChild1Table Table_dreamlessRow::Child1Empty;
    
    Table_dreamlessChild_newsprintTable Table_dreamlessRow::Child_newsprintEmpty;
    
    Table_dreamlessChild_nevusTable Table_dreamlessRow::Child_nevusEmpty;
    
    Table_dreamlessChild3Table Table_dreamlessRow::Child3Empty;
    
    Table_dreamlessChild2Table Table_dreamlessRow::Child2Empty;
    
    Table_dreamlessRow::Table_dreamlessRow(CremaReader::irow& row, Table_dreamlessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->operetta = row.to_int64(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_dreamlessRow::Child1Empty);
        }
        if ((this->Child_newsprint == nullptr))
        {
            this->Child_newsprint = &(Table_dreamlessRow::Child_newsprintEmpty);
        }
        if ((this->Child_nevus == nullptr))
        {
            this->Child_nevus = &(Table_dreamlessRow::Child_nevusEmpty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_dreamlessRow::Child3Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_dreamlessRow::Child2Empty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table_dreamlessSetChild1(Table_dreamlessRow* target, const std::vector<Table_dreamlessChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_dreamlessChild1Table(childs);
    }
    
    void Table_dreamlessSetChild_newsprint(Table_dreamlessRow* target, const std::vector<Table_dreamlessChild_newsprintRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_newsprint = new Table_dreamlessChild_newsprintTable(childs);
    }
    
    void Table_dreamlessSetChild_nevus(Table_dreamlessRow* target, const std::vector<Table_dreamlessChild_nevusRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_nevus = new Table_dreamlessChild_nevusTable(childs);
    }
    
    void Table_dreamlessSetChild3(Table_dreamlessRow* target, const std::vector<Table_dreamlessChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_dreamlessChild3Table(childs);
    }
    
    void Table_dreamlessSetChild2(Table_dreamlessRow* target, const std::vector<Table_dreamlessChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_dreamlessChild2Table(childs);
    }
    
    Table_dreamlessTable::Table_dreamlessTable()
    {
    }
    
    Table_dreamlessTable::Table_dreamlessTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_dreamlessChild1Table(table.dataset().tables()["Table_dreamless.Child1"]);
        this->Child_newsprint = new Table_dreamlessChild_newsprintTable(table.dataset().tables()["Table_dreamless.Child_newsprint"]);
        this->Child_nevus = new Table_dreamlessChild_nevusTable(table.dataset().tables()["Table_dreamless.Child_nevus"]);
        this->Child3 = new Table_dreamlessChild3Table(table.dataset().tables()["Table_dreamless.Child3"]);
        this->Child2 = new Table_dreamlessChild2Table(table.dataset().tables()["Table_dreamless.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_dreamlessSetChild1);
        this->SetRelations(this->Child_newsprint->Rows, Table_dreamlessSetChild_newsprint);
        this->SetRelations(this->Child_nevus->Rows, Table_dreamlessSetChild_nevus);
        this->SetRelations(this->Child3->Rows, Table_dreamlessSetChild3);
        this->SetRelations(this->Child2->Rows, Table_dreamlessSetChild2);
    }
    
    Table_dreamlessTable::~Table_dreamlessTable()
    {
        delete this->Child1;
        delete this->Child_newsprint;
        delete this->Child_nevus;
        delete this->Child3;
        delete this->Child2;
    }
    
    void* Table_dreamlessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_dreamlessRow(row, ((Table_dreamlessTable*)(table)));
    }
    
    const Table_dreamlessRow* Table_dreamlessTable::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table81Row::Table81Row(CremaReader::irow& row, Table81Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->drowse = ((Type39)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Galaxy = row.to_uint8(1);
        }
        this->propelled = row.to_uint8(2);
        if (row.has_value(3))
        {
            this->colic = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->excavator = ((Type_nephew)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->Malthusian = row.to_uint16(5);
        }
        if (row.has_value(6))
        {
            this->yapped = ((Type21)(row.to_int32(6)));
        }
        this->SetKey(this->drowse, this->propelled);
    }
    
    Table81Table::Table81Table()
    {
    }
    
    Table81Table::Table81Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table81Table::~Table81Table()
    {
    }
    
    void* Table81Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table81Row(row, ((Table81Table*)(table)));
    }
    
    const Table81Row* Table81Table::Find(Type39 drowse, unsigned char propelled) const
    {
        return this->FindRow(drowse, propelled);
    }
    
    Table_chicaneRow::Table_chicaneRow(CremaReader::irow& row, Table_chicaneTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->brushfire = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->corpulence = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->dammed = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->Golgotha = row.to_uint16(3);
        }
        if (row.has_value(4))
        {
            this->planarity = row.to_int8(4);
        }
        this->SetKey(this->brushfire);
    }
    
    Table_chicaneTable::Table_chicaneTable()
    {
    }
    
    Table_chicaneTable::Table_chicaneTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_chicaneTable::~Table_chicaneTable()
    {
    }
    
    void* Table_chicaneTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_chicaneRow(row, ((Table_chicaneTable*)(table)));
    }
    
    const Table_chicaneRow* Table_chicaneTable::Find(time_t brushfire) const
    {
        return this->FindRow(brushfire);
    }
    
    Table90Row::Table90Row(CremaReader::irow& row, Table90Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Presbyterian = row.to_datetime(0);
        this->SetKey(this->Presbyterian);
    }
    
    Table90Table::Table90Table()
    {
    }
    
    Table90Table::Table90Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table90Table::~Table90Table()
    {
    }
    
    void* Table90Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table90Row(row, ((Table90Table*)(table)));
    }
    
    const Table90Row* Table90Table::Find(time_t Presbyterian) const
    {
        return this->FindRow(Presbyterian);
    }
    
    Table203Row::Table203Row(CremaReader::irow& row, Table203Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->clamminess = row.to_string(0);
        if (row.has_value(1))
        {
            this->epigrapher = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Sebastian = row.to_boolean(2);
        }
        this->branned = row.to_datetime(3);
        if (row.has_value(4))
        {
            this->necessity = row.to_int32(4);
        }
        this->SetKey(this->clamminess, this->branned);
    }
    
    Table203Table::Table203Table()
    {
    }
    
    Table203Table::Table203Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table203Table::~Table203Table()
    {
    }
    
    void* Table203Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table203Row(row, ((Table203Table*)(table)));
    }
    
    const Table203Row* Table203Table::Find(const std::string& clamminess, time_t branned) const
    {
        return this->FindRow(clamminess, branned);
    }
    
    Table_attachmentRow::Table_attachmentRow(CremaReader::irow& row, Table_attachmentTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->payed = row.to_int16(0);
        if (row.has_value(1))
        {
            this->Neville = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->loom = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->laxativeness = row.to_string(3);
        }
        this->SetKey(this->payed);
    }
    
    Table_attachmentTable::Table_attachmentTable()
    {
    }
    
    Table_attachmentTable::Table_attachmentTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_attachmentTable::~Table_attachmentTable()
    {
    }
    
    void* Table_attachmentTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_attachmentRow(row, ((Table_attachmentTable*)(table)));
    }
    
    const Table_attachmentRow* Table_attachmentTable::Find(short payed) const
    {
        return this->FindRow(payed);
    }
    
    Table195Row::Table195Row(CremaReader::irow& row, Table195Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->mediation = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->neophyte = ((Type23)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->office = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->balaclava = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->Margarete = row.to_uint64(4);
        }
        this->SetKey(this->mediation);
    }
    
    Table195Table::Table195Table()
    {
    }
    
    Table195Table::Table195Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table195Table::~Table195Table()
    {
    }
    
    void* Table195Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table195Row(row, ((Table195Table*)(table)));
    }
    
    const Table195Row* Table195Table::Find(unsigned char mediation) const
    {
        return this->FindRow(mediation);
    }
    
    Table_PamirsRow::Table_PamirsRow(CremaReader::irow& row, Table_PamirsTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->misstep = ((Type8)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Alistair = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->birth = ((Type15)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Abdel = row.to_int16(3);
        }
        if (row.has_value(4))
        {
            this->cousinly = row.to_uint64(4);
        }
        if (row.has_value(5))
        {
            this->Paley = row.to_int16(5);
        }
        this->SetKey(this->misstep);
    }
    
    Table_PamirsTable::Table_PamirsTable()
    {
    }
    
    Table_PamirsTable::Table_PamirsTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_PamirsTable::~Table_PamirsTable()
    {
    }
    
    void* Table_PamirsTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_PamirsRow(row, ((Table_PamirsTable*)(table)));
    }
    
    const Table_PamirsRow* Table_PamirsTable::Find(Type8 misstep) const
    {
        return this->FindRow(misstep);
    }
    
    Table_kinderRow::Table_kinderRow(CremaReader::irow& row, Table_kinderTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->typewriter = row.to_string(0);
        if (row.has_value(1))
        {
            this->instituter = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->outhit = row.to_string(2);
        }
        this->SetKey(this->typewriter);
    }
    
    Table_kinderTable::Table_kinderTable()
    {
    }
    
    Table_kinderTable::Table_kinderTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_kinderTable::~Table_kinderTable()
    {
    }
    
    void* Table_kinderTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_kinderRow(row, ((Table_kinderTable*)(table)));
    }
    
    const Table_kinderRow* Table_kinderTable::Find(const std::string& typewriter) const
    {
        return this->FindRow(typewriter);
    }
    
    Table_deathlessRow::Table_deathlessRow(CremaReader::irow& row, Table_deathlessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lumberyard = ((Type6)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Carl = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->firearm = row.to_boolean(2);
        }
        this->uncap = row.to_int8(3);
        if (row.has_value(4))
        {
            this->chairwoman = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->Bassett = ((Type8)(row.to_int32(5)));
        }
        this->SetKey(this->lumberyard, this->uncap);
    }
    
    Table_deathlessTable::Table_deathlessTable()
    {
    }
    
    Table_deathlessTable::Table_deathlessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_deathlessTable::~Table_deathlessTable()
    {
    }
    
    void* Table_deathlessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_deathlessRow(row, ((Table_deathlessTable*)(table)));
    }
    
    const Table_deathlessRow* Table_deathlessTable::Find(Type6 lumberyard, char uncap) const
    {
        return this->FindRow(lumberyard, uncap);
    }
    
    Table117Row::Table117Row(CremaReader::irow& row, Table117Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->paranormal = row.to_string(0);
        if (row.has_value(1))
        {
            this->kipping = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->receptionist = ((Type21)(row.to_int32(2)));
        }
        this->SetKey(this->paranormal);
    }
    
    Table117Table::Table117Table()
    {
    }
    
    Table117Table::Table117Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table117Table::~Table117Table()
    {
    }
    
    void* Table117Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table117Row(row, ((Table117Table*)(table)));
    }
    
    const Table117Row* Table117Table::Find(const std::string& paranormal) const
    {
        return this->FindRow(paranormal);
    }
    
    Table_selectionRow::Table_selectionRow(CremaReader::irow& row, Table_selectionTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->seemliness = row.to_single(0);
        if (row.has_value(1))
        {
            this->sandpit = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->carbonization = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->whacker = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->diluteness = row.to_uint64(4);
        }
        if (row.has_value(5))
        {
            this->happing = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->considerable = row.to_int64(6);
        }
        if (row.has_value(7))
        {
            this->mis = row.to_double(7);
        }
        this->SetKey(this->seemliness);
    }
    
    Table_selectionTable::Table_selectionTable()
    {
    }
    
    Table_selectionTable::Table_selectionTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_selectionTable::~Table_selectionTable()
    {
    }
    
    void* Table_selectionTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_selectionRow(row, ((Table_selectionTable*)(table)));
    }
    
    const Table_selectionRow* Table_selectionTable::Find(float seemliness) const
    {
        return this->FindRow(seemliness);
    }
    
    Table1Child1Row::Table1Child1Row(CremaReader::irow& row, Table1Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->pill = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->canoeist = ((Type_livingness)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->blunder = row.to_int32(5);
        }
        if (row.has_value(6))
        {
            this->fastidious = row.to_datetime(6);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table1Child1Table::Table1Child1Table()
    {
    }
    
    Table1Child1Table::Table1Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table1Child1Table::Table1Child1Table(std::vector<Table1Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table1Child1Table::~Table1Child1Table()
    {
    }
    
    void* Table1Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table1Child1Row(row, ((Table1Child1Table*)(table)));
    }
    
    const Table1Child1Row* Table1Child1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table1Child2Row::Table1Child2Row(CremaReader::irow& row, Table1Child2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->eruption = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->busty = ((Type_Attn)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Glory = row.to_uint64(3);
        }
        this->SetKey(this->outgrip);
    }
    
    Table1Child2Table::Table1Child2Table()
    {
    }
    
    Table1Child2Table::Table1Child2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table1Child2Table::Table1Child2Table(std::vector<Table1Child2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table1Child2Table::~Table1Child2Table()
    {
    }
    
    void* Table1Child2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table1Child2Row(row, ((Table1Child2Table*)(table)));
    }
    
    const Table1Child2Row* Table1Child2Table::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table1Child_debatableRow::Table1Child_debatableRow(CremaReader::irow& row, Table1Child_debatableTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->dampen = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->hearing = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->bedsheets = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->Terri = row.to_int32(3);
        }
        this->Zagreb = row.to_boolean(4);
        if (row.has_value(5))
        {
            this->reading = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->leopardess = row.to_double(6);
        }
        this->SetKey(this->dampen, this->Zagreb);
    }
    
    Table1Child_debatableTable::Table1Child_debatableTable()
    {
    }
    
    Table1Child_debatableTable::Table1Child_debatableTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table1Child_debatableTable::Table1Child_debatableTable(std::vector<Table1Child_debatableRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table1Child_debatableTable::~Table1Child_debatableTable()
    {
    }
    
    void* Table1Child_debatableTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table1Child_debatableRow(row, ((Table1Child_debatableTable*)(table)));
    }
    
    const Table1Child_debatableRow* Table1Child_debatableTable::Find(unsigned long long dampen, bool Zagreb) const
    {
        return this->FindRow(dampen, Zagreb);
    }
    
    Table1Child1Table Table1Row::Child1Empty;
    
    Table1Child2Table Table1Row::Child2Empty;
    
    Table1Child_debatableTable Table1Row::Child_debatableEmpty;
    
    Table1Row::Table1Row(CremaReader::irow& row, Table1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->cytoplasm = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->Haddad = ((Type35)(row.to_int32(4)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table1Row::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table1Row::Child2Empty);
        }
        if ((this->Child_debatable == nullptr))
        {
            this->Child_debatable = &(Table1Row::Child_debatableEmpty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table1SetChild1(Table1Row* target, const std::vector<Table1Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table1Child1Table(childs);
    }
    
    void Table1SetChild2(Table1Row* target, const std::vector<Table1Child2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table1Child2Table(childs);
    }
    
    void Table1SetChild_debatable(Table1Row* target, const std::vector<Table1Child_debatableRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_debatable = new Table1Child_debatableTable(childs);
    }
    
    Table1Table::Table1Table()
    {
    }
    
    Table1Table::Table1Table(CremaReader::itable& table)
    {
        this->Child1 = new Table1Child1Table(table.dataset().tables()["Table1.Child1"]);
        this->Child2 = new Table1Child2Table(table.dataset().tables()["Table1.Child2"]);
        this->Child_debatable = new Table1Child_debatableTable(table.dataset().tables()["Table1.Child_debatable"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table1SetChild1);
        this->SetRelations(this->Child2->Rows, Table1SetChild2);
        this->SetRelations(this->Child_debatable->Rows, Table1SetChild_debatable);
    }
    
    Table1Table::~Table1Table()
    {
        delete this->Child1;
        delete this->Child2;
        delete this->Child_debatable;
    }
    
    void* Table1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table1Row(row, ((Table1Table*)(table)));
    }
    
    const Table1Row* Table1Table::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table68Row::Table68Row(CremaReader::irow& row, Table68Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->tempura = row.to_uint32(0);
        this->SetKey(this->tempura);
    }
    
    Table68Table::Table68Table()
    {
    }
    
    Table68Table::Table68Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table68Table::~Table68Table()
    {
    }
    
    void* Table68Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table68Row(row, ((Table68Table*)(table)));
    }
    
    const Table68Row* Table68Table::Find(unsigned int tempura) const
    {
        return this->FindRow(tempura);
    }
    
    Table_emotionlessRow::Table_emotionlessRow(CremaReader::irow& row, Table_emotionlessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Nilsson = ((Type_Gretta)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->serendipitous = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->Pawtucket = row.to_duration(2);
        }
        this->krone = row.to_string(3);
        this->SetKey(this->Nilsson, this->krone);
    }
    
    Table_emotionlessTable::Table_emotionlessTable()
    {
    }
    
    Table_emotionlessTable::Table_emotionlessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_emotionlessTable::~Table_emotionlessTable()
    {
    }
    
    void* Table_emotionlessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_emotionlessRow(row, ((Table_emotionlessTable*)(table)));
    }
    
    const Table_emotionlessRow* Table_emotionlessTable::Find(Type_Gretta Nilsson, const std::string& krone) const
    {
        return this->FindRow(Nilsson, krone);
    }
    
    Table_halterRow::Table_halterRow(CremaReader::irow& row, Table_halterTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->snapback = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->caseworker = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Adonis = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->borderer = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->rattling = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->South = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->wagging = row.to_uint32(6);
        }
        if (row.has_value(7))
        {
            this->deleterious = row.to_duration(7);
        }
        if (row.has_value(8))
        {
            this->Dalmatian = row.to_int64(8);
        }
        if (row.has_value(9))
        {
            this->daysack = row.to_datetime(9);
        }
        if (row.has_value(10))
        {
            this->sabbath = row.to_int8(10);
        }
        this->SetKey(this->snapback);
    }
    
    Table_halterTable::Table_halterTable()
    {
    }
    
    Table_halterTable::Table_halterTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_halterTable::~Table_halterTable()
    {
    }
    
    void* Table_halterTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_halterRow(row, ((Table_halterTable*)(table)));
    }
    
    const Table_halterRow* Table_halterTable::Find(unsigned int snapback) const
    {
        return this->FindRow(snapback);
    }
    
    Table_studioRow::Table_studioRow(CremaReader::irow& row, Table_studioTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Midwest = row.to_double(0);
        if (row.has_value(1))
        {
            this->locale = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->aim = ((Type_Madison)(row.to_int32(2)));
        }
        this->outfielder = row.to_string(3);
        if (row.has_value(4))
        {
            this->suitableness = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->British = row.to_int32(5);
        }
        if (row.has_value(6))
        {
            this->broadcast = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->injurer = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->revelry = row.to_string(8);
        }
        if (row.has_value(9))
        {
            this->Congo = row.to_single(9);
        }
        if (row.has_value(10))
        {
            this->thrall = row.to_string(10);
        }
        this->SetKey(this->Midwest, this->outfielder);
    }
    
    Table_studioTable::Table_studioTable()
    {
    }
    
    Table_studioTable::Table_studioTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_studioTable::~Table_studioTable()
    {
    }
    
    void* Table_studioTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_studioRow(row, ((Table_studioTable*)(table)));
    }
    
    const Table_studioRow* Table_studioTable::Find(double Midwest, const std::string& outfielder) const
    {
        return this->FindRow(Midwest, outfielder);
    }
    
    Table115Row::Table115Row(CremaReader::irow& row, Table115Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Pawtucket = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->leatherneck = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->polymorphic = ((Type_consortia)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->rollicking = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->remodel = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->minster = row.to_int64(5);
        }
        this->SetKey(this->Pawtucket);
    }
    
    Table115Table::Table115Table()
    {
    }
    
    Table115Table::Table115Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table115Table::~Table115Table()
    {
    }
    
    void* Table115Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table115Row(row, ((Table115Table*)(table)));
    }
    
    const Table115Row* Table115Table::Find(unsigned int Pawtucket) const
    {
        return this->FindRow(Pawtucket);
    }
    
    Table182Child1Row::Table182Child1Row(CremaReader::irow& row, Table182Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->divalent = row.to_int64(0);
        if (row.has_value(1))
        {
            this->thoughtlessness = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->dextrose = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->belladonna = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->sadden = ((Type80)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->clef = row.to_int32(5);
        }
        if (row.has_value(6))
        {
            this->footlights = ((Type_Gretta)(row.to_int32(6)));
        }
        this->SetKey(this->divalent);
    }
    
    Table182Child1Table::Table182Child1Table()
    {
    }
    
    Table182Child1Table::Table182Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table182Child1Table::Table182Child1Table(std::vector<Table182Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table182Child1Table::~Table182Child1Table()
    {
    }
    
    void* Table182Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table182Child1Row(row, ((Table182Child1Table*)(table)));
    }
    
    const Table182Child1Row* Table182Child1Table::Find(long long divalent) const
    {
        return this->FindRow(divalent);
    }
    
    Table182Child1Table Table182Row::Child1Empty;
    
    Table182Row::Table182Row(CremaReader::irow& row, Table182Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Borden = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->Mikkel = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->underadjusting = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->transcendence = row.to_uint8(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table182Row::Child1Empty);
        }
        this->SetKey(this->Borden);
    }
    
    void Table182SetChild1(Table182Row* target, const std::vector<Table182Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table182Child1Table(childs);
    }
    
    Table182Table::Table182Table()
    {
    }
    
    Table182Table::Table182Table(CremaReader::itable& table)
    {
        this->Child1 = new Table182Child1Table(table.dataset().tables()["Table182.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table182SetChild1);
    }
    
    Table182Table::~Table182Table()
    {
        delete this->Child1;
    }
    
    void* Table182Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table182Row(row, ((Table182Table*)(table)));
    }
    
    const Table182Row* Table182Table::Find(unsigned int Borden) const
    {
        return this->FindRow(Borden);
    }
    
    Table_sunRow::Table_sunRow(CremaReader::irow& row, Table_sunTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->unnavigable = row.to_single(0);
        if (row.has_value(1))
        {
            this->Jarad = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->separates = row.to_int64(2);
        }
        this->assiduity = row.to_datetime(3);
        if (row.has_value(4))
        {
            this->yardmaster = ((Type8)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->Sir = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->thermostat = ((Type_Attn)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->Gregoor = row.to_string(7);
        }
        if (row.has_value(8))
        {
            this->planetarium = row.to_uint16(8);
        }
        this->SetKey(this->unnavigable, this->assiduity);
    }
    
    Table_sunTable::Table_sunTable()
    {
    }
    
    Table_sunTable::Table_sunTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_sunTable::~Table_sunTable()
    {
    }
    
    void* Table_sunTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_sunRow(row, ((Table_sunTable*)(table)));
    }
    
    const Table_sunRow* Table_sunTable::Find(float unnavigable, time_t assiduity) const
    {
        return this->FindRow(unnavigable, assiduity);
    }
    
    Table_licenseRow::Table_licenseRow(CremaReader::irow& row, Table_licenseTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->ultimateness = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->erosive = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->tackler = row.to_duration(2);
        }
        this->Glass = row.to_uint32(3);
        if (row.has_value(4))
        {
            this->amylase = row.to_single(4);
        }
        this->impose = row.to_string(5);
        this->SetKey(this->ultimateness, this->Glass, this->impose);
    }
    
    Table_licenseTable::Table_licenseTable()
    {
    }
    
    Table_licenseTable::Table_licenseTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_licenseTable::~Table_licenseTable()
    {
    }
    
    void* Table_licenseTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_licenseRow(row, ((Table_licenseTable*)(table)));
    }
    
    const Table_licenseRow* Table_licenseTable::Find(time_t ultimateness, unsigned int Glass, const std::string& impose) const
    {
        return this->FindRow(ultimateness, Glass, impose);
    }
    
    Table129Row::Table129Row(CremaReader::irow& row, Table129Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->periodical = row.to_string(0);
        if (row.has_value(1))
        {
            this->Lorri = ((Type27)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->construe = row.to_uint64(2);
        }
        this->SetKey(this->periodical);
    }
    
    Table129Table::Table129Table()
    {
    }
    
    Table129Table::Table129Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table129Table::~Table129Table()
    {
    }
    
    void* Table129Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table129Row(row, ((Table129Table*)(table)));
    }
    
    const Table129Row* Table129Table::Find(const std::string& periodical) const
    {
        return this->FindRow(periodical);
    }
    
    Table_navigableChild1Row::Table_navigableChild1Row(CremaReader::irow& row, Table_navigableChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->woodlouse = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Letitia = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->Livonia = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->Praia = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->Christmas = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->earner = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->forwent = row.to_duration(6);
        }
        if (row.has_value(7))
        {
            this->dependability = row.to_uint32(7);
        }
        this->SetKey(this->woodlouse);
    }
    
    Table_navigableChild1Table::Table_navigableChild1Table()
    {
    }
    
    Table_navigableChild1Table::Table_navigableChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_navigableChild1Table::Table_navigableChild1Table(std::vector<Table_navigableChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_navigableChild1Table::~Table_navigableChild1Table()
    {
    }
    
    void* Table_navigableChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_navigableChild1Row(row, ((Table_navigableChild1Table*)(table)));
    }
    
    const Table_navigableChild1Row* Table_navigableChild1Table::Find(unsigned short woodlouse) const
    {
        return this->FindRow(woodlouse);
    }
    
    Table_navigableChild2Row::Table_navigableChild2Row(CremaReader::irow& row, Table_navigableChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Lynn = row.to_int8(0);
        if (row.has_value(1))
        {
            this->clause = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->scrupulosity = row.to_int16(2);
        }
        this->abbrev = row.to_uint16(3);
        if (row.has_value(4))
        {
            this->microdot = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->Estes = ((Type_Multan)(row.to_int32(5)));
        }
        this->SetKey(this->Lynn, this->abbrev);
    }
    
    Table_navigableChild2Table::Table_navigableChild2Table()
    {
    }
    
    Table_navigableChild2Table::Table_navigableChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_navigableChild2Table::Table_navigableChild2Table(std::vector<Table_navigableChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_navigableChild2Table::~Table_navigableChild2Table()
    {
    }
    
    void* Table_navigableChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_navigableChild2Row(row, ((Table_navigableChild2Table*)(table)));
    }
    
    const Table_navigableChild2Row* Table_navigableChild2Table::Find(char Lynn, unsigned short abbrev) const
    {
        return this->FindRow(Lynn, abbrev);
    }
    
    Table_navigableChild_tapiocaRow::Table_navigableChild_tapiocaRow(CremaReader::irow& row, Table_navigableChild_tapiocaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->illegality = row.to_uint8(0);
        this->metricize = row.to_uint32(1);
        if (row.has_value(2))
        {
            this->bong = row.to_string(2);
        }
        this->confound = row.to_uint32(3);
        if (row.has_value(4))
        {
            this->coprophagous = row.to_boolean(4);
        }
        this->rosin = row.to_uint32(5);
        this->SetKey(this->illegality, this->metricize, this->confound, this->rosin);
    }
    
    Table_navigableChild_tapiocaTable::Table_navigableChild_tapiocaTable()
    {
    }
    
    Table_navigableChild_tapiocaTable::Table_navigableChild_tapiocaTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_navigableChild_tapiocaTable::Table_navigableChild_tapiocaTable(std::vector<Table_navigableChild_tapiocaRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_navigableChild_tapiocaTable::~Table_navigableChild_tapiocaTable()
    {
    }
    
    void* Table_navigableChild_tapiocaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_navigableChild_tapiocaRow(row, ((Table_navigableChild_tapiocaTable*)(table)));
    }
    
    const Table_navigableChild_tapiocaRow* Table_navigableChild_tapiocaTable::Find(unsigned char illegality, unsigned int metricize, unsigned int confound, unsigned int rosin) const
    {
        return this->FindRow(illegality, metricize, confound, rosin);
    }
    
    Table_navigableChild1Table Table_navigableRow::Child1Empty;
    
    Table_navigableChild2Table Table_navigableRow::Child2Empty;
    
    Table_navigableChild_tapiocaTable Table_navigableRow::Child_tapiocaEmpty;
    
    Table_navigableRow::Table_navigableRow(CremaReader::irow& row, Table_navigableTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->multiplicative = ((Type_Multan)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->math = ((Type_Meiji)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->rickshaw = ((Type_Meiji)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->dimmed = ((Type_Arlan)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->bucker = row.to_double(4);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_navigableRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_navigableRow::Child2Empty);
        }
        if ((this->Child_tapioca == nullptr))
        {
            this->Child_tapioca = &(Table_navigableRow::Child_tapiocaEmpty);
        }
        this->SetKey(this->multiplicative);
    }
    
    void Table_navigableSetChild1(Table_navigableRow* target, const std::vector<Table_navigableChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_navigableChild1Table(childs);
    }
    
    void Table_navigableSetChild2(Table_navigableRow* target, const std::vector<Table_navigableChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_navigableChild2Table(childs);
    }
    
    void Table_navigableSetChild_tapioca(Table_navigableRow* target, const std::vector<Table_navigableChild_tapiocaRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_tapioca = new Table_navigableChild_tapiocaTable(childs);
    }
    
    Table_navigableTable::Table_navigableTable()
    {
    }
    
    Table_navigableTable::Table_navigableTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_navigableChild1Table(table.dataset().tables()["Table_navigable.Child1"]);
        this->Child2 = new Table_navigableChild2Table(table.dataset().tables()["Table_navigable.Child2"]);
        this->Child_tapioca = new Table_navigableChild_tapiocaTable(table.dataset().tables()["Table_navigable.Child_tapioca"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_navigableSetChild1);
        this->SetRelations(this->Child2->Rows, Table_navigableSetChild2);
        this->SetRelations(this->Child_tapioca->Rows, Table_navigableSetChild_tapioca);
    }
    
    Table_navigableTable::~Table_navigableTable()
    {
        delete this->Child1;
        delete this->Child2;
        delete this->Child_tapioca;
    }
    
    void* Table_navigableTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_navigableRow(row, ((Table_navigableTable*)(table)));
    }
    
    const Table_navigableRow* Table_navigableTable::Find(Type_Multan multiplicative) const
    {
        return this->FindRow(multiplicative);
    }
    
    Table_anchorRow::Table_anchorRow(CremaReader::irow& row, Table_anchorTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lumberyard = ((Type6)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Carl = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->firearm = row.to_boolean(2);
        }
        this->uncap = row.to_int8(3);
        if (row.has_value(4))
        {
            this->chairwoman = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->Bassett = ((Type8)(row.to_int32(5)));
        }
        this->SetKey(this->lumberyard, this->uncap);
    }
    
    Table_anchorTable::Table_anchorTable()
    {
    }
    
    Table_anchorTable::Table_anchorTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_anchorTable::~Table_anchorTable()
    {
    }
    
    void* Table_anchorTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_anchorRow(row, ((Table_anchorTable*)(table)));
    }
    
    const Table_anchorRow* Table_anchorTable::Find(Type6 lumberyard, char uncap) const
    {
        return this->FindRow(lumberyard, uncap);
    }
    
    Table_LeontineRow::Table_LeontineRow(CremaReader::irow& row, Table_LeontineTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->referral = row.to_double(0);
        if (row.has_value(1))
        {
            this->Tana = ((Type3)(row.to_int32(1)));
        }
        this->baryon = ((Type4)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->Shawnee = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->ostensibly = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->Chrissie = row.to_string(5);
        }
        this->needlewoman = row.to_string(6);
        this->SetKey(this->referral, this->baryon, this->needlewoman);
    }
    
    Table_LeontineTable::Table_LeontineTable()
    {
    }
    
    Table_LeontineTable::Table_LeontineTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LeontineTable::~Table_LeontineTable()
    {
    }
    
    void* Table_LeontineTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LeontineRow(row, ((Table_LeontineTable*)(table)));
    }
    
    const Table_LeontineRow* Table_LeontineTable::Find(double referral, Type4 baryon, const std::string& needlewoman) const
    {
        return this->FindRow(referral, baryon, needlewoman);
    }
    
    Table149Row::Table149Row(CremaReader::irow& row, Table149Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->typewriter = row.to_string(0);
        if (row.has_value(1))
        {
            this->instituter = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->outhit = row.to_string(2);
        }
        this->SetKey(this->typewriter);
    }
    
    Table149Table::Table149Table()
    {
    }
    
    Table149Table::Table149Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table149Table::~Table149Table()
    {
    }
    
    void* Table149Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table149Row(row, ((Table149Table*)(table)));
    }
    
    const Table149Row* Table149Table::Find(const std::string& typewriter) const
    {
        return this->FindRow(typewriter);
    }
    
    Table17Row::Table17Row(CremaReader::irow& row, Table17Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Tanny = row.to_double(0);
        if (row.has_value(1))
        {
            this->locale = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->fagoting = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->aim = ((Type_Madison)(row.to_int32(3)));
        }
        this->brutality = row.to_string(4);
        if (row.has_value(5))
        {
            this->scantly = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->British = row.to_int32(6);
        }
        if (row.has_value(7))
        {
            this->broadcast = row.to_int8(7);
        }
        if (row.has_value(8))
        {
            this->injurer = row.to_single(8);
        }
        this->SetKey(this->Tanny, this->brutality);
    }
    
    Table17Table::Table17Table()
    {
    }
    
    Table17Table::Table17Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table17Table::~Table17Table()
    {
    }
    
    void* Table17Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table17Row(row, ((Table17Table*)(table)));
    }
    
    const Table17Row* Table17Table::Find(double Tanny, const std::string& brutality) const
    {
        return this->FindRow(Tanny, brutality);
    }
    
    Table_hoodlumChild_smuttyDeletableRow::Table_hoodlumChild_smuttyDeletableRow(CremaReader::irow& row, Table_hoodlumChild_smuttyDeletableTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->stranglehold = row.to_single(0);
        if (row.has_value(1))
        {
            this->affluent = ((Type1)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->photostatic = row.to_uint16(2);
        }
        if (row.has_value(3))
        {
            this->toadstool = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->schnauzer = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->predation = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->Ade = ((Type_rifled)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->gag = row.to_duration(7);
        }
        if (row.has_value(8))
        {
            this->sequoia = row.to_int64(8);
        }
        if (row.has_value(9))
        {
            this->somerset = row.to_int64(9);
        }
        if (row.has_value(10))
        {
            this->diffusion = ((Type_RhodesDeletable)(row.to_int32(10)));
        }
        if (row.has_value(11))
        {
            this->haggler = row.to_string(11);
        }
        this->SetKey(this->stranglehold);
    }
    
    Table_hoodlumChild_smuttyDeletableTable::Table_hoodlumChild_smuttyDeletableTable()
    {
    }
    
    Table_hoodlumChild_smuttyDeletableTable::Table_hoodlumChild_smuttyDeletableTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_hoodlumChild_smuttyDeletableTable::Table_hoodlumChild_smuttyDeletableTable(std::vector<Table_hoodlumChild_smuttyDeletableRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_hoodlumChild_smuttyDeletableTable::~Table_hoodlumChild_smuttyDeletableTable()
    {
    }
    
    void* Table_hoodlumChild_smuttyDeletableTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_hoodlumChild_smuttyDeletableRow(row, ((Table_hoodlumChild_smuttyDeletableTable*)(table)));
    }
    
    const Table_hoodlumChild_smuttyDeletableRow* Table_hoodlumChild_smuttyDeletableTable::Find(float stranglehold) const
    {
        return this->FindRow(stranglehold);
    }
    
    Table_hoodlumChild_smuttyDeletableTable Table_hoodlumRow::Child_smuttyDeletableEmpty;
    
    Table_hoodlumRow::Table_hoodlumRow(CremaReader::irow& row, Table_hoodlumTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Portsmouth = row.to_int16(0);
        if (row.has_value(1))
        {
            this->bout = ((Type_rennet)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->aerosol = ((Type_HeraclitusDeletable)(row.to_int32(2)));
        }
        if ((this->Child_smuttyDeletable == nullptr))
        {
            this->Child_smuttyDeletable = &(Table_hoodlumRow::Child_smuttyDeletableEmpty);
        }
        this->SetKey(this->Portsmouth);
    }
    
    void Table_hoodlumSetChild_smuttyDeletable(Table_hoodlumRow* target, const std::vector<Table_hoodlumChild_smuttyDeletableRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_smuttyDeletable = new Table_hoodlumChild_smuttyDeletableTable(childs);
    }
    
    Table_hoodlumTable::Table_hoodlumTable()
    {
    }
    
    Table_hoodlumTable::Table_hoodlumTable(CremaReader::itable& table)
    {
        this->Child_smuttyDeletable = new Table_hoodlumChild_smuttyDeletableTable(table.dataset().tables()["Table_hoodlum.Child_smuttyDeletable"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_smuttyDeletable->Rows, Table_hoodlumSetChild_smuttyDeletable);
    }
    
    Table_hoodlumTable::~Table_hoodlumTable()
    {
        delete this->Child_smuttyDeletable;
    }
    
    void* Table_hoodlumTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_hoodlumRow(row, ((Table_hoodlumTable*)(table)));
    }
    
    const Table_hoodlumRow* Table_hoodlumTable::Find(short Portsmouth) const
    {
        return this->FindRow(Portsmouth);
    }
    
    Table138Row::Table138Row(CremaReader::irow& row, Table138Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->cartographer = row.to_int16(0);
        if (row.has_value(1))
        {
            this->dastardly = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->rimless = row.to_int16(2);
        }
        this->views = ((Type21)(row.to_int32(3)));
        this->grubbed = row.to_uint8(4);
        if (row.has_value(5))
        {
            this->vapid = row.to_uint16(5);
        }
        if (row.has_value(6))
        {
            this->antigenic = row.to_single(6);
        }
        this->ethnicity = row.to_int8(7);
        if (row.has_value(8))
        {
            this->assimilate = ((Type_insolent)(row.to_int32(8)));
        }
        this->SetKey(this->cartographer, this->views, this->grubbed, this->ethnicity);
    }
    
    Table138Table::Table138Table()
    {
    }
    
    Table138Table::Table138Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table138Table::~Table138Table()
    {
    }
    
    void* Table138Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table138Row(row, ((Table138Table*)(table)));
    }
    
    const Table138Row* Table138Table::Find(short cartographer, Type21 views, unsigned char grubbed, char ethnicity) const
    {
        return this->FindRow(cartographer, views, grubbed, ethnicity);
    }
    
    Table88Row::Table88Row(CremaReader::irow& row, Table88Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Seabrook = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Garry = ((Type_consortia)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->thereof = row.to_uint64(2);
        }
        if (row.has_value(3))
        {
            this->further = row.to_int32(3);
        }
        this->issuer = row.to_double(4);
        if (row.has_value(5))
        {
            this->breeding = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->dander = ((Type12)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->Masada = ((Type_canted)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->impressible = row.to_int16(8);
        }
        if (row.has_value(9))
        {
            this->endurably = row.to_single(9);
        }
        if (row.has_value(10))
        {
            this->Burty = ((Type_Jenelle)(row.to_int32(10)));
        }
        this->SetKey(this->Seabrook, this->issuer);
    }
    
    Table88Table::Table88Table()
    {
    }
    
    Table88Table::Table88Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table88Table::~Table88Table()
    {
    }
    
    void* Table88Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table88Row(row, ((Table88Table*)(table)));
    }
    
    const Table88Row* Table88Table::Find(unsigned short Seabrook, double issuer) const
    {
        return this->FindRow(Seabrook, issuer);
    }
    
    Table_BonneeRow::Table_BonneeRow(CremaReader::irow& row, Table_BonneeTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Portsmouth = row.to_int16(0);
        if (row.has_value(1))
        {
            this->bout = ((Type_rennet)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->overexpose = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->leotard = row.to_uint16(3);
        }
        this->SetKey(this->Portsmouth);
    }
    
    Table_BonneeTable::Table_BonneeTable()
    {
    }
    
    Table_BonneeTable::Table_BonneeTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_BonneeTable::~Table_BonneeTable()
    {
    }
    
    void* Table_BonneeTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_BonneeRow(row, ((Table_BonneeTable*)(table)));
    }
    
    const Table_BonneeRow* Table_BonneeTable::Find(short Portsmouth) const
    {
        return this->FindRow(Portsmouth);
    }
    
    Table_JoycelinRow::Table_JoycelinRow(CremaReader::irow& row, Table_JoycelinTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->brushfire = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->corpulence = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->dammed = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->Golgotha = row.to_uint16(3);
        }
        if (row.has_value(4))
        {
            this->planarity = row.to_int8(4);
        }
        this->SetKey(this->brushfire);
    }
    
    Table_JoycelinTable::Table_JoycelinTable()
    {
    }
    
    Table_JoycelinTable::Table_JoycelinTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_JoycelinTable::~Table_JoycelinTable()
    {
    }
    
    void* Table_JoycelinTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_JoycelinRow(row, ((Table_JoycelinTable*)(table)));
    }
    
    const Table_JoycelinRow* Table_JoycelinTable::Find(time_t brushfire) const
    {
        return this->FindRow(brushfire);
    }
    
    Table_reticulateChild1Row::Table_reticulateChild1Row(CremaReader::irow& row, Table_reticulateChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->warhead = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->champ = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->relaxedness = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->calorimetric = ((Type_rifled)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->rhesus = row.to_uint16(4);
        }
        if (row.has_value(5))
        {
            this->Runyon = ((Type_spokespeople)(row.to_int32(5)));
        }
        this->SetKey(this->warhead);
    }
    
    Table_reticulateChild1Table::Table_reticulateChild1Table()
    {
    }
    
    Table_reticulateChild1Table::Table_reticulateChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_reticulateChild1Table::Table_reticulateChild1Table(std::vector<Table_reticulateChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_reticulateChild1Table::~Table_reticulateChild1Table()
    {
    }
    
    void* Table_reticulateChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_reticulateChild1Row(row, ((Table_reticulateChild1Table*)(table)));
    }
    
    const Table_reticulateChild1Row* Table_reticulateChild1Table::Find(bool warhead) const
    {
        return this->FindRow(warhead);
    }
    
    Table_reticulateChild2Row::Table_reticulateChild2Row(CremaReader::irow& row, Table_reticulateChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Ward = row.to_int16(0);
        if (row.has_value(1))
        {
            this->writeup = row.to_int16(1);
        }
        this->SetKey(this->Ward);
    }
    
    Table_reticulateChild2Table::Table_reticulateChild2Table()
    {
    }
    
    Table_reticulateChild2Table::Table_reticulateChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_reticulateChild2Table::Table_reticulateChild2Table(std::vector<Table_reticulateChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_reticulateChild2Table::~Table_reticulateChild2Table()
    {
    }
    
    void* Table_reticulateChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_reticulateChild2Row(row, ((Table_reticulateChild2Table*)(table)));
    }
    
    const Table_reticulateChild2Row* Table_reticulateChild2Table::Find(short Ward) const
    {
        return this->FindRow(Ward);
    }
    
    Table_reticulateChild1Table Table_reticulateRow::Child1Empty;
    
    Table_reticulateChild2Table Table_reticulateRow::Child2Empty;
    
    Table_reticulateRow::Table_reticulateRow(CremaReader::irow& row, Table_reticulateTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->immediacy = row.to_int32(0);
        if (row.has_value(1))
        {
            this->dateline = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->setup = ((Type_Gretta)(row.to_int32(2)));
        }
        this->lettering = row.to_double(3);
        this->colatitude = ((Type_Multan)(row.to_int32(4)));
        if (row.has_value(5))
        {
            this->Sam = row.to_int8(5);
        }
        this->maleficence = row.to_single(6);
        if (row.has_value(7))
        {
            this->Fermi = ((Type1)(row.to_int32(7)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_reticulateRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_reticulateRow::Child2Empty);
        }
        this->SetKey(this->immediacy, this->lettering, this->colatitude, this->maleficence);
    }
    
    void Table_reticulateSetChild1(Table_reticulateRow* target, const std::vector<Table_reticulateChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_reticulateChild1Table(childs);
    }
    
    void Table_reticulateSetChild2(Table_reticulateRow* target, const std::vector<Table_reticulateChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_reticulateChild2Table(childs);
    }
    
    Table_reticulateTable::Table_reticulateTable()
    {
    }
    
    Table_reticulateTable::Table_reticulateTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_reticulateChild1Table(table.dataset().tables()["Table_reticulate.Child1"]);
        this->Child2 = new Table_reticulateChild2Table(table.dataset().tables()["Table_reticulate.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_reticulateSetChild1);
        this->SetRelations(this->Child2->Rows, Table_reticulateSetChild2);
    }
    
    Table_reticulateTable::~Table_reticulateTable()
    {
        delete this->Child1;
        delete this->Child2;
    }
    
    void* Table_reticulateTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_reticulateRow(row, ((Table_reticulateTable*)(table)));
    }
    
    const Table_reticulateRow* Table_reticulateTable::Find(int immediacy, double lettering, Type_Multan colatitude, float maleficence) const
    {
        return this->FindRow(immediacy, lettering, colatitude, maleficence);
    }
    
    Table114Row::Table114Row(CremaReader::irow& row, Table114Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Elie = row.to_duration(0);
        if (row.has_value(1))
        {
            this->traction = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->cartload = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Cartwright = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->handshaking = row.to_uint16(4);
        }
        if (row.has_value(5))
        {
            this->Timex = row.to_duration(5);
        }
        this->SetKey(this->Elie);
    }
    
    Table114Table::Table114Table()
    {
    }
    
    Table114Table::Table114Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table114Table::~Table114Table()
    {
    }
    
    void* Table114Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table114Row(row, ((Table114Table*)(table)));
    }
    
    const Table114Row* Table114Table::Find(int Elie) const
    {
        return this->FindRow(Elie);
    }
    
    Table118Row::Table118Row(CremaReader::irow& row, Table118Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->romantically = row.to_uint8(0);
        this->interwoven = row.to_single(1);
        if (row.has_value(2))
        {
            this->speed = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->Provencals = row.to_uint8(3);
        }
        if (row.has_value(4))
        {
            this->biologic = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->volcanoes = row.to_duration(5);
        }
        this->extendedness = ((Type30)(row.to_int32(6)));
        if (row.has_value(7))
        {
            this->Renaud = ((Type60)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->satisfactory = row.to_single(8);
        }
        if (row.has_value(9))
        {
            this->coursework = row.to_uint32(9);
        }
        this->potentiating = row.to_datetime(10);
        this->SetKey(this->romantically, this->interwoven, this->extendedness, this->potentiating);
    }
    
    Table118Table::Table118Table()
    {
    }
    
    Table118Table::Table118Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table118Table::~Table118Table()
    {
    }
    
    void* Table118Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table118Row(row, ((Table118Table*)(table)));
    }
    
    const Table118Row* Table118Table::Find(unsigned char romantically, float interwoven, Type30 extendedness, time_t potentiating) const
    {
        return this->FindRow(romantically, interwoven, extendedness, potentiating);
    }
    
    Table170Row::Table170Row(CremaReader::irow& row, Table170Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->excise = row.to_int8(0);
        if (row.has_value(1))
        {
            this->trio = row.to_boolean(1);
        }
        this->inquiry = row.to_boolean(2);
        if (row.has_value(3))
        {
            this->concealment = ((Type_hyperboloidal)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->cornfield = ((Type77)(row.to_int32(4)));
        }
        this->SetKey(this->excise, this->inquiry);
    }
    
    Table170Table::Table170Table()
    {
    }
    
    Table170Table::Table170Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table170Table::~Table170Table()
    {
    }
    
    void* Table170Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table170Row(row, ((Table170Table*)(table)));
    }
    
    const Table170Row* Table170Table::Find(char excise, bool inquiry) const
    {
        return this->FindRow(excise, inquiry);
    }
    
    Table64Row::Table64Row(CremaReader::irow& row, Table64Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->furthermost = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->parliament = row.to_int32(1);
        }
        this->SetKey(this->furthermost);
    }
    
    Table64Table::Table64Table()
    {
    }
    
    Table64Table::Table64Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table64Table::~Table64Table()
    {
    }
    
    void* Table64Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table64Row(row, ((Table64Table*)(table)));
    }
    
    const Table64Row* Table64Table::Find(bool furthermost) const
    {
        return this->FindRow(furthermost);
    }
    
    Table_designedRow::Table_designedRow(CremaReader::irow& row, Table_designedTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Tanny = row.to_double(0);
        if (row.has_value(1))
        {
            this->locale = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->fagoting = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->aim = ((Type_Madison)(row.to_int32(3)));
        }
        this->brutality = row.to_string(4);
        if (row.has_value(5))
        {
            this->scantly = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->British = row.to_int32(6);
        }
        if (row.has_value(7))
        {
            this->broadcast = row.to_int8(7);
        }
        if (row.has_value(8))
        {
            this->injurer = row.to_single(8);
        }
        this->SetKey(this->Tanny, this->brutality);
    }
    
    Table_designedTable::Table_designedTable()
    {
    }
    
    Table_designedTable::Table_designedTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_designedTable::~Table_designedTable()
    {
    }
    
    void* Table_designedTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_designedRow(row, ((Table_designedTable*)(table)));
    }
    
    const Table_designedRow* Table_designedTable::Find(double Tanny, const std::string& brutality) const
    {
        return this->FindRow(Tanny, brutality);
    }
    
    Table_scoutRow::Table_scoutRow(CremaReader::irow& row, Table_scoutTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->turk = row.to_single(0);
        if (row.has_value(1))
        {
            this->blankness = ((Type74)(row.to_int32(1)));
        }
        this->SetKey(this->turk);
    }
    
    Table_scoutTable::Table_scoutTable()
    {
    }
    
    Table_scoutTable::Table_scoutTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_scoutTable::~Table_scoutTable()
    {
    }
    
    void* Table_scoutTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_scoutRow(row, ((Table_scoutTable*)(table)));
    }
    
    const Table_scoutRow* Table_scoutTable::Find(float turk) const
    {
        return this->FindRow(turk);
    }
    
    Table_summarizerChild_WinnetkaRow::Table_summarizerChild_WinnetkaRow(CremaReader::irow& row, Table_summarizerChild_WinnetkaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->exudation = row.to_duration(0);
        this->SetKey(this->exudation);
    }
    
    Table_summarizerChild_WinnetkaTable::Table_summarizerChild_WinnetkaTable()
    {
    }
    
    Table_summarizerChild_WinnetkaTable::Table_summarizerChild_WinnetkaTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_summarizerChild_WinnetkaTable::Table_summarizerChild_WinnetkaTable(std::vector<Table_summarizerChild_WinnetkaRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_summarizerChild_WinnetkaTable::~Table_summarizerChild_WinnetkaTable()
    {
    }
    
    void* Table_summarizerChild_WinnetkaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_summarizerChild_WinnetkaRow(row, ((Table_summarizerChild_WinnetkaTable*)(table)));
    }
    
    const Table_summarizerChild_WinnetkaRow* Table_summarizerChild_WinnetkaTable::Find(int exudation) const
    {
        return this->FindRow(exudation);
    }
    
    Table_summarizerChild_WinnetkaTable Table_summarizerRow::Child_WinnetkaEmpty;
    
    Table_summarizerRow::Table_summarizerRow(CremaReader::irow& row, Table_summarizerTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->corpsman = row.to_double(0);
        if (row.has_value(1))
        {
            this->Araucanian = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Kaposi = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->hyperemia = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->pensiveness = row.to_single(4);
        }
        this->jetting = row.to_boolean(5);
        if (row.has_value(6))
        {
            this->babe = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->clears = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->codetermine = row.to_duration(8);
        }
        if ((this->Child_Winnetka == nullptr))
        {
            this->Child_Winnetka = &(Table_summarizerRow::Child_WinnetkaEmpty);
        }
        this->SetKey(this->corpsman, this->jetting);
    }
    
    void Table_summarizerSetChild_Winnetka(Table_summarizerRow* target, const std::vector<Table_summarizerChild_WinnetkaRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Winnetka = new Table_summarizerChild_WinnetkaTable(childs);
    }
    
    Table_summarizerTable::Table_summarizerTable()
    {
    }
    
    Table_summarizerTable::Table_summarizerTable(CremaReader::itable& table)
    {
        this->Child_Winnetka = new Table_summarizerChild_WinnetkaTable(table.dataset().tables()["Table_summarizer.Child_Winnetka"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Winnetka->Rows, Table_summarizerSetChild_Winnetka);
    }
    
    Table_summarizerTable::~Table_summarizerTable()
    {
        delete this->Child_Winnetka;
    }
    
    void* Table_summarizerTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_summarizerRow(row, ((Table_summarizerTable*)(table)));
    }
    
    const Table_summarizerRow* Table_summarizerTable::Find(double corpsman, bool jetting) const
    {
        return this->FindRow(corpsman, jetting);
    }
    
    Table132Row::Table132Row(CremaReader::irow& row, Table132Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->sexy = row.to_datetime(0);
        this->SetKey(this->sexy);
    }
    
    Table132Table::Table132Table()
    {
    }
    
    Table132Table::Table132Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table132Table::~Table132Table()
    {
    }
    
    void* Table132Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table132Row(row, ((Table132Table*)(table)));
    }
    
    const Table132Row* Table132Table::Find(time_t sexy) const
    {
        return this->FindRow(sexy);
    }
    
    Table133Row::Table133Row(CremaReader::irow& row, Table133Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Teresita = ((Type3)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->bloody = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->deerstalker = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->overrule = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->stripped = row.to_uint64(4);
        }
        if (row.has_value(5))
        {
            this->baler = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->Alikee = row.to_boolean(6);
        }
        if (row.has_value(7))
        {
            this->windup = row.to_string(7);
        }
        if (row.has_value(8))
        {
            this->drizzling = ((Type_Arlan)(row.to_int32(8)));
        }
        this->SetKey(this->Teresita);
    }
    
    Table133Table::Table133Table()
    {
    }
    
    Table133Table::Table133Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table133Table::~Table133Table()
    {
    }
    
    void* Table133Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table133Row(row, ((Table133Table*)(table)));
    }
    
    const Table133Row* Table133Table::Find(Type3 Teresita) const
    {
        return this->FindRow(Teresita);
    }
    
    Table154Row::Table154Row(CremaReader::irow& row, Table154Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->qualification = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->bite = row.to_duration(1);
        }
        this->SetKey(this->qualification);
    }
    
    Table154Table::Table154Table()
    {
    }
    
    Table154Table::Table154Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table154Table::~Table154Table()
    {
    }
    
    void* Table154Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table154Row(row, ((Table154Table*)(table)));
    }
    
    const Table154Row* Table154Table::Find(unsigned short qualification) const
    {
        return this->FindRow(qualification);
    }
    
    Table_fittedRow::Table_fittedRow(CremaReader::irow& row, Table_fittedTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outmaneuver = row.to_int64(0);
        if (row.has_value(1))
        {
            this->Heddi = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->sidle = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->kWh = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->toothpick = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->overstate = ((Type13)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->schoolyard = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->Tuareg = row.to_int16(7);
        }
        if (row.has_value(8))
        {
            this->Sennett = row.to_double(8);
        }
        if (row.has_value(9))
        {
            this->deicer = row.to_double(9);
        }
        if (row.has_value(10))
        {
            this->prefect = row.to_int64(10);
        }
        this->SetKey(this->outmaneuver);
    }
    
    Table_fittedTable::Table_fittedTable()
    {
    }
    
    Table_fittedTable::Table_fittedTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_fittedTable::~Table_fittedTable()
    {
    }
    
    void* Table_fittedTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_fittedRow(row, ((Table_fittedTable*)(table)));
    }
    
    const Table_fittedRow* Table_fittedTable::Find(long long outmaneuver) const
    {
        return this->FindRow(outmaneuver);
    }
    
    Table_TupungatoChild1Row::Table_TupungatoChild1Row(CremaReader::irow& row, Table_TupungatoChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->parabolic = row.to_int32(0);
        if (row.has_value(1))
        {
            this->syncopation = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->steamroller = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->purloiner = row.to_int64(3);
        }
        this->rattrap = row.to_int16(4);
        if (row.has_value(5))
        {
            this->Redondo = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->peach = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->asbestos = row.to_int64(7);
        }
        if (row.has_value(8))
        {
            this->glazing = row.to_uint16(8);
        }
        this->SetKey(this->parabolic, this->rattrap);
    }
    
    Table_TupungatoChild1Table::Table_TupungatoChild1Table()
    {
    }
    
    Table_TupungatoChild1Table::Table_TupungatoChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_TupungatoChild1Table::Table_TupungatoChild1Table(std::vector<Table_TupungatoChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_TupungatoChild1Table::~Table_TupungatoChild1Table()
    {
    }
    
    void* Table_TupungatoChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_TupungatoChild1Row(row, ((Table_TupungatoChild1Table*)(table)));
    }
    
    const Table_TupungatoChild1Row* Table_TupungatoChild1Table::Find(int parabolic, short rattrap) const
    {
        return this->FindRow(parabolic, rattrap);
    }
    
    Table_TupungatoChild1Table Table_TupungatoRow::Child1Empty;
    
    Table_TupungatoRow::Table_TupungatoRow(CremaReader::irow& row, Table_TupungatoTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->marginalia = row.to_uint64(0);
        this->chatted = row.to_int64(1);
        if (row.has_value(2))
        {
            this->dutiful = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->strangulate = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->Rozella = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->patrician = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->brunet = row.to_uint16(6);
        }
        if (row.has_value(7))
        {
            this->sweeping = row.to_int16(7);
        }
        if (row.has_value(8))
        {
            this->priesthood = row.to_int32(8);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_TupungatoRow::Child1Empty);
        }
        this->SetKey(this->marginalia, this->chatted);
    }
    
    void Table_TupungatoSetChild1(Table_TupungatoRow* target, const std::vector<Table_TupungatoChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_TupungatoChild1Table(childs);
    }
    
    Table_TupungatoTable::Table_TupungatoTable()
    {
    }
    
    Table_TupungatoTable::Table_TupungatoTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_TupungatoChild1Table(table.dataset().tables()["Table_Tupungato.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_TupungatoSetChild1);
    }
    
    Table_TupungatoTable::~Table_TupungatoTable()
    {
        delete this->Child1;
    }
    
    void* Table_TupungatoTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_TupungatoRow(row, ((Table_TupungatoTable*)(table)));
    }
    
    const Table_TupungatoRow* Table_TupungatoTable::Find(unsigned long long marginalia, long long chatted) const
    {
        return this->FindRow(marginalia, chatted);
    }
    
    Table130Row::Table130Row(CremaReader::irow& row, Table130Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->duplicative = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->unfailing = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->Sherman = row.to_int32(2);
        }
        this->SetKey(this->duplicative);
    }
    
    Table130Table::Table130Table()
    {
    }
    
    Table130Table::Table130Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table130Table::~Table130Table()
    {
    }
    
    void* Table130Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table130Row(row, ((Table130Table*)(table)));
    }
    
    const Table130Row* Table130Table::Find(unsigned char duplicative) const
    {
        return this->FindRow(duplicative);
    }
    
    Table200Row::Table200Row(CremaReader::irow& row, Table200Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->paranoia = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->vanquish = row.to_uint16(1);
        }
        this->bonkers = row.to_uint32(2);
        if (row.has_value(3))
        {
            this->spinal = row.to_boolean(3);
        }
        this->lagging = row.to_int32(4);
        if (row.has_value(5))
        {
            this->anywise = row.to_uint32(5);
        }
        this->SetKey(this->paranoia, this->bonkers, this->lagging);
    }
    
    Table200Table::Table200Table()
    {
    }
    
    Table200Table::Table200Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table200Table::~Table200Table()
    {
    }
    
    void* Table200Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table200Row(row, ((Table200Table*)(table)));
    }
    
    const Table200Row* Table200Table::Find(unsigned int paranoia, unsigned int bonkers, int lagging) const
    {
        return this->FindRow(paranoia, bonkers, lagging);
    }
    
    Table_capsulizeRow::Table_capsulizeRow(CremaReader::irow& row, Table_capsulizeTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->trenchermen = ((Type_hand)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->limit = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->gratified = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->Yokohama = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->Blaire = row.to_int16(4);
        }
        if (row.has_value(5))
        {
            this->polyclinic = ((Type40)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->concerned = row.to_int16(6);
        }
        this->SetKey(this->trenchermen);
    }
    
    Table_capsulizeTable::Table_capsulizeTable()
    {
    }
    
    Table_capsulizeTable::Table_capsulizeTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_capsulizeTable::~Table_capsulizeTable()
    {
    }
    
    void* Table_capsulizeTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_capsulizeRow(row, ((Table_capsulizeTable*)(table)));
    }
    
    const Table_capsulizeRow* Table_capsulizeTable::Find(Type_hand trenchermen) const
    {
        return this->FindRow(trenchermen);
    }
    
    Table210Row::Table210Row(CremaReader::irow& row, Table210Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Pollard = row.to_duration(0);
        if (row.has_value(1))
        {
            this->Ardisj = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->goodish = row.to_boolean(2);
        }
        this->thoroughness = row.to_int16(3);
        if (row.has_value(4))
        {
            this->Andie = row.to_uint16(4);
        }
        if (row.has_value(5))
        {
            this->paternalism = row.to_int32(5);
        }
        this->SetKey(this->Pollard, this->thoroughness);
    }
    
    Table210Table::Table210Table()
    {
    }
    
    Table210Table::Table210Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table210Table::~Table210Table()
    {
    }
    
    void* Table210Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table210Row(row, ((Table210Table*)(table)));
    }
    
    const Table210Row* Table210Table::Find(int Pollard, short thoroughness) const
    {
        return this->FindRow(Pollard, thoroughness);
    }
    
    Table192Row::Table192Row(CremaReader::irow& row, Table192Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->clumsy = row.to_uint16(0);
        this->SetKey(this->clumsy);
    }
    
    Table192Table::Table192Table()
    {
    }
    
    Table192Table::Table192Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table192Table::~Table192Table()
    {
    }
    
    void* Table192Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table192Row(row, ((Table192Table*)(table)));
    }
    
    const Table192Row* Table192Table::Find(unsigned short clumsy) const
    {
        return this->FindRow(clumsy);
    }
    
    Table_adulthoodChild1Row::Table_adulthoodChild1Row(CremaReader::irow& row, Table_adulthoodChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->florin = ((Type43)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->amazon = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->featherbed = ((Type30)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Dumont = row.to_uint8(3);
        }
        if (row.has_value(4))
        {
            this->malevolencies = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->weighs = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->Crowley = row.to_string(6);
        }
        if (row.has_value(7))
        {
            this->Ewell = ((Type_nephew)(row.to_int32(7)));
        }
        this->SetKey(this->florin);
    }
    
    Table_adulthoodChild1Table::Table_adulthoodChild1Table()
    {
    }
    
    Table_adulthoodChild1Table::Table_adulthoodChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_adulthoodChild1Table::Table_adulthoodChild1Table(std::vector<Table_adulthoodChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_adulthoodChild1Table::~Table_adulthoodChild1Table()
    {
    }
    
    void* Table_adulthoodChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_adulthoodChild1Row(row, ((Table_adulthoodChild1Table*)(table)));
    }
    
    const Table_adulthoodChild1Row* Table_adulthoodChild1Table::Find(Type43 florin) const
    {
        return this->FindRow(florin);
    }
    
    Table_adulthoodChild1Table Table_adulthoodRow::Child1Empty;
    
    Table_adulthoodRow::Table_adulthoodRow(CremaReader::irow& row, Table_adulthoodTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->may = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->whiten = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Romans = ((Type25)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Appalachian = ((Type13)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->Denmark = row.to_uint16(4);
        }
        if (row.has_value(5))
        {
            this->Merrie = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->premeditated = row.to_int16(6);
        }
        if (row.has_value(7))
        {
            this->Berzelius = ((Type_rennet)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->Angelia = ((Type_Attn)(row.to_int32(8)));
        }
        if (row.has_value(9))
        {
            this->erupt = ((Type50)(row.to_int32(9)));
        }
        if (row.has_value(10))
        {
            this->Yetty = row.to_datetime(10);
        }
        if (row.has_value(11))
        {
            this->PMS = row.to_int16(11);
        }
        if (row.has_value(12))
        {
            this->sharia = row.to_uint64(12);
        }
        if (row.has_value(13))
        {
            this->inflexion = row.to_int64(13);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_adulthoodRow::Child1Empty);
        }
        this->SetKey(this->may);
    }
    
    void Table_adulthoodSetChild1(Table_adulthoodRow* target, const std::vector<Table_adulthoodChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_adulthoodChild1Table(childs);
    }
    
    Table_adulthoodTable::Table_adulthoodTable()
    {
    }
    
    Table_adulthoodTable::Table_adulthoodTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_adulthoodChild1Table(table.dataset().tables()["Table_adulthood.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_adulthoodSetChild1);
    }
    
    Table_adulthoodTable::~Table_adulthoodTable()
    {
        delete this->Child1;
    }
    
    void* Table_adulthoodTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_adulthoodRow(row, ((Table_adulthoodTable*)(table)));
    }
    
    const Table_adulthoodRow* Table_adulthoodTable::Find(unsigned char may) const
    {
        return this->FindRow(may);
    }
    
    Table_gerrymanderRow::Table_gerrymanderRow(CremaReader::irow& row, Table_gerrymanderTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->unnavigable = row.to_single(0);
        if (row.has_value(1))
        {
            this->Jarad = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->separates = row.to_int64(2);
        }
        this->assiduity = row.to_datetime(3);
        if (row.has_value(4))
        {
            this->yardmaster = ((Type8)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->Sir = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->thermostat = ((Type_Attn)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->Gregoor = row.to_string(7);
        }
        if (row.has_value(8))
        {
            this->planetarium = row.to_uint16(8);
        }
        this->SetKey(this->unnavigable, this->assiduity);
    }
    
    Table_gerrymanderTable::Table_gerrymanderTable()
    {
    }
    
    Table_gerrymanderTable::Table_gerrymanderTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_gerrymanderTable::~Table_gerrymanderTable()
    {
    }
    
    void* Table_gerrymanderTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_gerrymanderRow(row, ((Table_gerrymanderTable*)(table)));
    }
    
    const Table_gerrymanderRow* Table_gerrymanderTable::Find(float unnavigable, time_t assiduity) const
    {
        return this->FindRow(unnavigable, assiduity);
    }
    
    Table193Row::Table193Row(CremaReader::irow& row, Table193Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->sisterliness = row.to_uint8(0);
        this->SetKey(this->sisterliness);
    }
    
    Table193Table::Table193Table()
    {
    }
    
    Table193Table::Table193Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table193Table::~Table193Table()
    {
    }
    
    void* Table193Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table193Row(row, ((Table193Table*)(table)));
    }
    
    const Table193Row* Table193Table::Find(unsigned char sisterliness) const
    {
        return this->FindRow(sisterliness);
    }
    
    Table_needlessnessRow::Table_needlessnessRow(CremaReader::irow& row, Table_needlessnessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Portsmouth = row.to_int16(0);
        if (row.has_value(1))
        {
            this->bout = ((Type_rennet)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->bowlful = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->ABS = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->bellflower = row.to_int16(4);
        }
        if (row.has_value(5))
        {
            this->preoccupation = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->nonsensitive = ((Type_spokespeople)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->implicant = row.to_int64(7);
        }
        if (row.has_value(8))
        {
            this->qty = row.to_int32(8);
        }
        if (row.has_value(9))
        {
            this->guile = row.to_int16(9);
        }
        if (row.has_value(10))
        {
            this->talebearer = row.to_single(10);
        }
        this->SetKey(this->Portsmouth);
    }
    
    Table_needlessnessTable::Table_needlessnessTable()
    {
    }
    
    Table_needlessnessTable::Table_needlessnessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_needlessnessTable::~Table_needlessnessTable()
    {
    }
    
    void* Table_needlessnessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_needlessnessRow(row, ((Table_needlessnessTable*)(table)));
    }
    
    const Table_needlessnessRow* Table_needlessnessTable::Find(short Portsmouth) const
    {
        return this->FindRow(Portsmouth);
    }
    
    Table24Row::Table24Row(CremaReader::irow& row, Table24Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Dina = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->conviction = row.to_int64(1);
        }
        if (row.has_value(2))
        {
            this->Bellanca = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->chancing = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->definer = row.to_duration(4);
        }
        this->SetKey(this->Dina);
    }
    
    Table24Table::Table24Table()
    {
    }
    
    Table24Table::Table24Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table24Table::~Table24Table()
    {
    }
    
    void* Table24Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table24Row(row, ((Table24Table*)(table)));
    }
    
    const Table24Row* Table24Table::Find(unsigned long long Dina) const
    {
        return this->FindRow(Dina);
    }
    
    Table62Row::Table62Row(CremaReader::irow& row, Table62Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->contract = ((Type12)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->tile = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->rimless = row.to_int64(2);
        }
        this->SetKey(this->contract);
    }
    
    Table62Table::Table62Table()
    {
    }
    
    Table62Table::Table62Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table62Table::~Table62Table()
    {
    }
    
    void* Table62Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table62Row(row, ((Table62Table*)(table)));
    }
    
    const Table62Row* Table62Table::Find(Type12 contract) const
    {
        return this->FindRow(contract);
    }
    
    Table_expansionaryChild_KatrinkaRow::Table_expansionaryChild_KatrinkaRow(CremaReader::irow& row, Table_expansionaryChild_KatrinkaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Patti = row.to_duration(0);
        if (row.has_value(1))
        {
            this->glazed = row.to_double(1);
        }
        this->diffract = row.to_boolean(2);
        if (row.has_value(3))
        {
            this->calamitous = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->Cranmer = row.to_int32(4);
        }
        if (row.has_value(5))
        {
            this->Volvo = row.to_duration(5);
        }
        this->SetKey(this->Patti, this->diffract);
    }
    
    Table_expansionaryChild_KatrinkaTable::Table_expansionaryChild_KatrinkaTable()
    {
    }
    
    Table_expansionaryChild_KatrinkaTable::Table_expansionaryChild_KatrinkaTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_expansionaryChild_KatrinkaTable::Table_expansionaryChild_KatrinkaTable(std::vector<Table_expansionaryChild_KatrinkaRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_expansionaryChild_KatrinkaTable::~Table_expansionaryChild_KatrinkaTable()
    {
    }
    
    void* Table_expansionaryChild_KatrinkaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_expansionaryChild_KatrinkaRow(row, ((Table_expansionaryChild_KatrinkaTable*)(table)));
    }
    
    const Table_expansionaryChild_KatrinkaRow* Table_expansionaryChild_KatrinkaTable::Find(int Patti, bool diffract) const
    {
        return this->FindRow(Patti, diffract);
    }
    
    Table_expansionaryChild_KatrinkaTable Table_expansionaryRow::Child_KatrinkaEmpty;
    
    Table_expansionaryRow::Table_expansionaryRow(CremaReader::irow& row, Table_expansionaryTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->snapback = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->caseworker = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Adonis = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->borderer = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->trollish = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->lukewarm = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->South = row.to_uint8(6);
        }
        if (row.has_value(7))
        {
            this->wagging = row.to_uint32(7);
        }
        if ((this->Child_Katrinka == nullptr))
        {
            this->Child_Katrinka = &(Table_expansionaryRow::Child_KatrinkaEmpty);
        }
        this->SetKey(this->snapback);
    }
    
    void Table_expansionarySetChild_Katrinka(Table_expansionaryRow* target, const std::vector<Table_expansionaryChild_KatrinkaRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Katrinka = new Table_expansionaryChild_KatrinkaTable(childs);
    }
    
    Table_expansionaryTable::Table_expansionaryTable()
    {
    }
    
    Table_expansionaryTable::Table_expansionaryTable(CremaReader::itable& table)
    {
        this->Child_Katrinka = new Table_expansionaryChild_KatrinkaTable(table.dataset().tables()["Table_expansionary.Child_Katrinka"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Katrinka->Rows, Table_expansionarySetChild_Katrinka);
    }
    
    Table_expansionaryTable::~Table_expansionaryTable()
    {
        delete this->Child_Katrinka;
    }
    
    void* Table_expansionaryTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_expansionaryRow(row, ((Table_expansionaryTable*)(table)));
    }
    
    const Table_expansionaryRow* Table_expansionaryTable::Find(unsigned int snapback) const
    {
        return this->FindRow(snapback);
    }
    
    Table_GiacintaChild1Row::Table_GiacintaChild1Row(CremaReader::irow& row, Table_GiacintaChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->schism = row.to_int64(0);
        if (row.has_value(1))
        {
            this->BBC = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->Ge = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->grubbed = row.to_int8(3);
        }
        this->SetKey(this->schism);
    }
    
    Table_GiacintaChild1Table::Table_GiacintaChild1Table()
    {
    }
    
    Table_GiacintaChild1Table::Table_GiacintaChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_GiacintaChild1Table::Table_GiacintaChild1Table(std::vector<Table_GiacintaChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_GiacintaChild1Table::~Table_GiacintaChild1Table()
    {
    }
    
    void* Table_GiacintaChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_GiacintaChild1Row(row, ((Table_GiacintaChild1Table*)(table)));
    }
    
    const Table_GiacintaChild1Row* Table_GiacintaChild1Table::Find(long long schism) const
    {
        return this->FindRow(schism);
    }
    
    Table_GiacintaChild2Row::Table_GiacintaChild2Row(CremaReader::irow& row, Table_GiacintaChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->gos = row.to_int8(0);
        if (row.has_value(1))
        {
            this->photolysis = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Liberian = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->Palmyra = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->airbus = row.to_datetime(4);
        }
        this->SetKey(this->gos);
    }
    
    Table_GiacintaChild2Table::Table_GiacintaChild2Table()
    {
    }
    
    Table_GiacintaChild2Table::Table_GiacintaChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_GiacintaChild2Table::Table_GiacintaChild2Table(std::vector<Table_GiacintaChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_GiacintaChild2Table::~Table_GiacintaChild2Table()
    {
    }
    
    void* Table_GiacintaChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_GiacintaChild2Row(row, ((Table_GiacintaChild2Table*)(table)));
    }
    
    const Table_GiacintaChild2Row* Table_GiacintaChild2Table::Find(char gos) const
    {
        return this->FindRow(gos);
    }
    
    Table_GiacintaChild1Table Table_GiacintaRow::Child1Empty;
    
    Table_GiacintaChild2Table Table_GiacintaRow::Child2Empty;
    
    Table_GiacintaRow::Table_GiacintaRow(CremaReader::irow& row, Table_GiacintaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->reprehensible = ((Type3)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->headmastership = row.to_datetime(1);
        }
        this->Gienah = row.to_int16(2);
        if (row.has_value(3))
        {
            this->capo = row.to_single(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_GiacintaRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_GiacintaRow::Child2Empty);
        }
        this->SetKey(this->reprehensible, this->Gienah);
    }
    
    void Table_GiacintaSetChild1(Table_GiacintaRow* target, const std::vector<Table_GiacintaChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_GiacintaChild1Table(childs);
    }
    
    void Table_GiacintaSetChild2(Table_GiacintaRow* target, const std::vector<Table_GiacintaChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_GiacintaChild2Table(childs);
    }
    
    Table_GiacintaTable::Table_GiacintaTable()
    {
    }
    
    Table_GiacintaTable::Table_GiacintaTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_GiacintaChild1Table(table.dataset().tables()["Table_Giacinta.Child1"]);
        this->Child2 = new Table_GiacintaChild2Table(table.dataset().tables()["Table_Giacinta.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_GiacintaSetChild1);
        this->SetRelations(this->Child2->Rows, Table_GiacintaSetChild2);
    }
    
    Table_GiacintaTable::~Table_GiacintaTable()
    {
        delete this->Child1;
        delete this->Child2;
    }
    
    void* Table_GiacintaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_GiacintaRow(row, ((Table_GiacintaTable*)(table)));
    }
    
    const Table_GiacintaRow* Table_GiacintaTable::Find(Type3 reprehensible, short Gienah) const
    {
        return this->FindRow(reprehensible, Gienah);
    }
    
    Table_IantheChild1Row::Table_IantheChild1Row(CremaReader::irow& row, Table_IantheChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->pill = row.to_boolean(3);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table_IantheChild1Table::Table_IantheChild1Table()
    {
    }
    
    Table_IantheChild1Table::Table_IantheChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_IantheChild1Table::Table_IantheChild1Table(std::vector<Table_IantheChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_IantheChild1Table::~Table_IantheChild1Table()
    {
    }
    
    void* Table_IantheChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_IantheChild1Row(row, ((Table_IantheChild1Table*)(table)));
    }
    
    const Table_IantheChild1Row* Table_IantheChild1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table_IantheChild2Row::Table_IantheChild2Row(CremaReader::irow& row, Table_IantheChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->eruption = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->busty = ((Type_Attn)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Glory = row.to_uint64(3);
        }
        this->SetKey(this->outgrip);
    }
    
    Table_IantheChild2Table::Table_IantheChild2Table()
    {
    }
    
    Table_IantheChild2Table::Table_IantheChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_IantheChild2Table::Table_IantheChild2Table(std::vector<Table_IantheChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_IantheChild2Table::~Table_IantheChild2Table()
    {
    }
    
    void* Table_IantheChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_IantheChild2Row(row, ((Table_IantheChild2Table*)(table)));
    }
    
    const Table_IantheChild2Row* Table_IantheChild2Table::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table_IantheChild3Row::Table_IantheChild3Row(CremaReader::irow& row, Table_IantheChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->dampen = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->hearing = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->bedsheets = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->Terri = row.to_int32(3);
        }
        this->Zagreb = row.to_boolean(4);
        if (row.has_value(5))
        {
            this->reading = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->leopardess = row.to_double(6);
        }
        this->SetKey(this->dampen, this->Zagreb);
    }
    
    Table_IantheChild3Table::Table_IantheChild3Table()
    {
    }
    
    Table_IantheChild3Table::Table_IantheChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_IantheChild3Table::Table_IantheChild3Table(std::vector<Table_IantheChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_IantheChild3Table::~Table_IantheChild3Table()
    {
    }
    
    void* Table_IantheChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_IantheChild3Row(row, ((Table_IantheChild3Table*)(table)));
    }
    
    const Table_IantheChild3Row* Table_IantheChild3Table::Find(unsigned long long dampen, bool Zagreb) const
    {
        return this->FindRow(dampen, Zagreb);
    }
    
    Table_IantheChild1Table Table_IantheRow::Child1Empty;
    
    Table_IantheChild2Table Table_IantheRow::Child2Empty;
    
    Table_IantheChild3Table Table_IantheRow::Child3Empty;
    
    Table_IantheRow::Table_IantheRow(CremaReader::irow& row, Table_IantheTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->operetta = row.to_int64(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_IantheRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_IantheRow::Child2Empty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_IantheRow::Child3Empty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table_IantheSetChild1(Table_IantheRow* target, const std::vector<Table_IantheChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_IantheChild1Table(childs);
    }
    
    void Table_IantheSetChild2(Table_IantheRow* target, const std::vector<Table_IantheChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_IantheChild2Table(childs);
    }
    
    void Table_IantheSetChild3(Table_IantheRow* target, const std::vector<Table_IantheChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_IantheChild3Table(childs);
    }
    
    Table_IantheTable::Table_IantheTable()
    {
    }
    
    Table_IantheTable::Table_IantheTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_IantheChild1Table(table.dataset().tables()["Table_Ianthe.Child1"]);
        this->Child2 = new Table_IantheChild2Table(table.dataset().tables()["Table_Ianthe.Child2"]);
        this->Child3 = new Table_IantheChild3Table(table.dataset().tables()["Table_Ianthe.Child3"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_IantheSetChild1);
        this->SetRelations(this->Child2->Rows, Table_IantheSetChild2);
        this->SetRelations(this->Child3->Rows, Table_IantheSetChild3);
    }
    
    Table_IantheTable::~Table_IantheTable()
    {
        delete this->Child1;
        delete this->Child2;
        delete this->Child3;
    }
    
    void* Table_IantheTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_IantheRow(row, ((Table_IantheTable*)(table)));
    }
    
    const Table_IantheRow* Table_IantheTable::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table_bootprintsChild1Row::Table_bootprintsChild1Row(CremaReader::irow& row, Table_bootprintsChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->Larsen = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->enforceable = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->aid = row.to_double(5);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table_bootprintsChild1Table::Table_bootprintsChild1Table()
    {
    }
    
    Table_bootprintsChild1Table::Table_bootprintsChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_bootprintsChild1Table::Table_bootprintsChild1Table(std::vector<Table_bootprintsChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_bootprintsChild1Table::~Table_bootprintsChild1Table()
    {
    }
    
    void* Table_bootprintsChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_bootprintsChild1Row(row, ((Table_bootprintsChild1Table*)(table)));
    }
    
    const Table_bootprintsChild1Row* Table_bootprintsChild1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table_bootprintsChild2Row::Table_bootprintsChild2Row(CremaReader::irow& row, Table_bootprintsChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->eruption = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->busty = ((Type_Attn)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Glory = row.to_uint64(3);
        }
        this->SetKey(this->outgrip);
    }
    
    Table_bootprintsChild2Table::Table_bootprintsChild2Table()
    {
    }
    
    Table_bootprintsChild2Table::Table_bootprintsChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_bootprintsChild2Table::Table_bootprintsChild2Table(std::vector<Table_bootprintsChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_bootprintsChild2Table::~Table_bootprintsChild2Table()
    {
    }
    
    void* Table_bootprintsChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_bootprintsChild2Row(row, ((Table_bootprintsChild2Table*)(table)));
    }
    
    const Table_bootprintsChild2Row* Table_bootprintsChild2Table::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table_bootprintsChild3Row::Table_bootprintsChild3Row(CremaReader::irow& row, Table_bootprintsChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->dampen = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->amuse = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->bedsheets = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->Terri = row.to_int32(3);
        }
        this->Zagreb = row.to_boolean(4);
        if (row.has_value(5))
        {
            this->leopardess = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->wrongheaded = row.to_string(6);
        }
        this->SetKey(this->dampen, this->Zagreb);
    }
    
    Table_bootprintsChild3Table::Table_bootprintsChild3Table()
    {
    }
    
    Table_bootprintsChild3Table::Table_bootprintsChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_bootprintsChild3Table::Table_bootprintsChild3Table(std::vector<Table_bootprintsChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_bootprintsChild3Table::~Table_bootprintsChild3Table()
    {
    }
    
    void* Table_bootprintsChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_bootprintsChild3Row(row, ((Table_bootprintsChild3Table*)(table)));
    }
    
    const Table_bootprintsChild3Row* Table_bootprintsChild3Table::Find(unsigned long long dampen, bool Zagreb) const
    {
        return this->FindRow(dampen, Zagreb);
    }
    
    Table_bootprintsChild1Table Table_bootprintsRow::Child1Empty;
    
    Table_bootprintsChild2Table Table_bootprintsRow::Child2Empty;
    
    Table_bootprintsChild3Table Table_bootprintsRow::Child3Empty;
    
    Table_bootprintsRow::Table_bootprintsRow(CremaReader::irow& row, Table_bootprintsTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->operetta = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->dammit = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->camp = row.to_uint64(5);
        }
        if (row.has_value(6))
        {
            this->Clint = row.to_uint64(6);
        }
        if (row.has_value(7))
        {
            this->marquee = row.to_int8(7);
        }
        if (row.has_value(8))
        {
            this->dunner = ((Type51)(row.to_int32(8)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_bootprintsRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_bootprintsRow::Child2Empty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_bootprintsRow::Child3Empty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table_bootprintsSetChild1(Table_bootprintsRow* target, const std::vector<Table_bootprintsChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_bootprintsChild1Table(childs);
    }
    
    void Table_bootprintsSetChild2(Table_bootprintsRow* target, const std::vector<Table_bootprintsChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_bootprintsChild2Table(childs);
    }
    
    void Table_bootprintsSetChild3(Table_bootprintsRow* target, const std::vector<Table_bootprintsChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_bootprintsChild3Table(childs);
    }
    
    Table_bootprintsTable::Table_bootprintsTable()
    {
    }
    
    Table_bootprintsTable::Table_bootprintsTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_bootprintsChild1Table(table.dataset().tables()["Table_bootprints.Child1"]);
        this->Child2 = new Table_bootprintsChild2Table(table.dataset().tables()["Table_bootprints.Child2"]);
        this->Child3 = new Table_bootprintsChild3Table(table.dataset().tables()["Table_bootprints.Child3"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_bootprintsSetChild1);
        this->SetRelations(this->Child2->Rows, Table_bootprintsSetChild2);
        this->SetRelations(this->Child3->Rows, Table_bootprintsSetChild3);
    }
    
    Table_bootprintsTable::~Table_bootprintsTable()
    {
        delete this->Child1;
        delete this->Child2;
        delete this->Child3;
    }
    
    void* Table_bootprintsTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_bootprintsRow(row, ((Table_bootprintsTable*)(table)));
    }
    
    const Table_bootprintsRow* Table_bootprintsTable::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table102Child1Row::Table102Child1Row(CremaReader::irow& row, Table102Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->composedness = row.to_double(0);
        if (row.has_value(1))
        {
            this->presoaks = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->Lodge = ((Type51)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Sumter = row.to_int16(3);
        }
        this->SetKey(this->composedness);
    }
    
    Table102Child1Table::Table102Child1Table()
    {
    }
    
    Table102Child1Table::Table102Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table102Child1Table::Table102Child1Table(std::vector<Table102Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table102Child1Table::~Table102Child1Table()
    {
    }
    
    void* Table102Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table102Child1Row(row, ((Table102Child1Table*)(table)));
    }
    
    const Table102Child1Row* Table102Child1Table::Find(double composedness) const
    {
        return this->FindRow(composedness);
    }
    
    Table102Child1Table Table102Row::Child1Empty;
    
    Table102Row::Table102Row(CremaReader::irow& row, Table102Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Mellisent = ((Type_Madison)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Galileo = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->Suzhou = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->Leanne = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->add = ((Type11)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->nodal = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->Malawi = ((Type_livingness)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->maria = row.to_uint16(7);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table102Row::Child1Empty);
        }
        this->SetKey(this->Mellisent);
    }
    
    void Table102SetChild1(Table102Row* target, const std::vector<Table102Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table102Child1Table(childs);
    }
    
    Table102Table::Table102Table()
    {
    }
    
    Table102Table::Table102Table(CremaReader::itable& table)
    {
        this->Child1 = new Table102Child1Table(table.dataset().tables()["Table102.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table102SetChild1);
    }
    
    Table102Table::~Table102Table()
    {
        delete this->Child1;
    }
    
    void* Table102Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table102Row(row, ((Table102Table*)(table)));
    }
    
    const Table102Row* Table102Table::Find(Type_Madison Mellisent) const
    {
        return this->FindRow(Mellisent);
    }
    
    Table164Row::Table164Row(CremaReader::irow& row, Table164Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->measles = ((Type_Dianna)(row.to_int32(0)));
        this->weaving = row.to_datetime(1);
        if (row.has_value(2))
        {
            this->Alexandrina = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->stakeholder = ((Type56)(row.to_int32(3)));
        }
        this->SetKey(this->measles, this->weaving);
    }
    
    Table164Table::Table164Table()
    {
    }
    
    Table164Table::Table164Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table164Table::~Table164Table()
    {
    }
    
    void* Table164Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table164Row(row, ((Table164Table*)(table)));
    }
    
    const Table164Row* Table164Table::Find(Type_Dianna measles, time_t weaving) const
    {
        return this->FindRow(measles, weaving);
    }
    
    Table120Row::Table120Row(CremaReader::irow& row, Table120Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->demagogy = row.to_duration(0);
        if (row.has_value(1))
        {
            this->promulgate = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->valley = row.to_int32(2);
        }
        if (row.has_value(3))
        {
            this->photojournalist = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->Elizabet = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->Antony = row.to_int16(5);
        }
        if (row.has_value(6))
        {
            this->obs = row.to_uint32(6);
        }
        this->SetKey(this->demagogy);
    }
    
    Table120Table::Table120Table()
    {
    }
    
    Table120Table::Table120Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table120Table::~Table120Table()
    {
    }
    
    void* Table120Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table120Row(row, ((Table120Table*)(table)));
    }
    
    const Table120Row* Table120Table::Find(int demagogy) const
    {
        return this->FindRow(demagogy);
    }
    
    Table158Child1Row::Table158Child1Row(CremaReader::irow& row, Table158Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Fragonard = row.to_duration(0);
        this->SetKey(this->Fragonard);
    }
    
    Table158Child1Table::Table158Child1Table()
    {
    }
    
    Table158Child1Table::Table158Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table158Child1Table::Table158Child1Table(std::vector<Table158Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table158Child1Table::~Table158Child1Table()
    {
    }
    
    void* Table158Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table158Child1Row(row, ((Table158Child1Table*)(table)));
    }
    
    const Table158Child1Row* Table158Child1Table::Find(int Fragonard) const
    {
        return this->FindRow(Fragonard);
    }
    
    Table158Child1Table Table158Row::Child1Empty;
    
    Table158Row::Table158Row(CremaReader::irow& row, Table158Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->differential = ((Type_Attn)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->coworker = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->polisher = row.to_int16(2);
        }
        if (row.has_value(3))
        {
            this->Kentucky = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->Kaleena = ((Type3)(row.to_int32(4)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table158Row::Child1Empty);
        }
        this->SetKey(this->differential);
    }
    
    void Table158SetChild1(Table158Row* target, const std::vector<Table158Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table158Child1Table(childs);
    }
    
    Table158Table::Table158Table()
    {
    }
    
    Table158Table::Table158Table(CremaReader::itable& table)
    {
        this->Child1 = new Table158Child1Table(table.dataset().tables()["Table158.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table158SetChild1);
    }
    
    Table158Table::~Table158Table()
    {
        delete this->Child1;
    }
    
    void* Table158Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table158Row(row, ((Table158Table*)(table)));
    }
    
    const Table158Row* Table158Table::Find(Type_Attn differential) const
    {
        return this->FindRow(differential);
    }
    
    Table_gynecologicChild1Row::Table_gynecologicChild1Row(CremaReader::irow& row, Table_gynecologicChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->humphs = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->beggarliness = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Judea = ((Type_ultrasonic)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->aerobically = row.to_int8(3);
        }
        if (row.has_value(4))
        {
            this->shredder = ((Type_rennet)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->Bresenham = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->bucksaw = ((Type42)(row.to_int32(6)));
        }
        this->SetKey(this->humphs);
    }
    
    Table_gynecologicChild1Table::Table_gynecologicChild1Table()
    {
    }
    
    Table_gynecologicChild1Table::Table_gynecologicChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_gynecologicChild1Table::Table_gynecologicChild1Table(std::vector<Table_gynecologicChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_gynecologicChild1Table::~Table_gynecologicChild1Table()
    {
    }
    
    void* Table_gynecologicChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_gynecologicChild1Row(row, ((Table_gynecologicChild1Table*)(table)));
    }
    
    const Table_gynecologicChild1Row* Table_gynecologicChild1Table::Find(time_t humphs) const
    {
        return this->FindRow(humphs);
    }
    
    Table_gynecologicChild1Table Table_gynecologicRow::Child1Empty;
    
    Table_gynecologicRow::Table_gynecologicRow(CremaReader::irow& row, Table_gynecologicTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Greer = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Celestine = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->hooter = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->postulate = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->jackhammer = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->lactic = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->swordsmen = ((Type8)(row.to_int32(6)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_gynecologicRow::Child1Empty);
        }
        this->SetKey(this->Greer);
    }
    
    void Table_gynecologicSetChild1(Table_gynecologicRow* target, const std::vector<Table_gynecologicChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_gynecologicChild1Table(childs);
    }
    
    Table_gynecologicTable::Table_gynecologicTable()
    {
    }
    
    Table_gynecologicTable::Table_gynecologicTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_gynecologicChild1Table(table.dataset().tables()["Table_gynecologic.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_gynecologicSetChild1);
    }
    
    Table_gynecologicTable::~Table_gynecologicTable()
    {
        delete this->Child1;
    }
    
    void* Table_gynecologicTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_gynecologicRow(row, ((Table_gynecologicTable*)(table)));
    }
    
    const Table_gynecologicRow* Table_gynecologicTable::Find(int Greer) const
    {
        return this->FindRow(Greer);
    }
    
    Table169Row::Table169Row(CremaReader::irow& row, Table169Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->toque = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->speller = row.to_single(1);
        }
        this->SetKey(this->toque);
    }
    
    Table169Table::Table169Table()
    {
    }
    
    Table169Table::Table169Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table169Table::~Table169Table()
    {
    }
    
    void* Table169Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table169Row(row, ((Table169Table*)(table)));
    }
    
    const Table169Row* Table169Table::Find(bool toque) const
    {
        return this->FindRow(toque);
    }
    
    Table6Child_frostbiteRow::Table6Child_frostbiteRow(CremaReader::irow& row, Table6Child_frostbiteTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Olag = row.to_int8(0);
        if (row.has_value(1))
        {
            this->taverner = ((Type_nephew)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->cowl = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->magnetite = ((Type23)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->acceptableness = row.to_int8(4);
        }
        this->SetKey(this->Olag);
    }
    
    Table6Child_frostbiteTable::Table6Child_frostbiteTable()
    {
    }
    
    Table6Child_frostbiteTable::Table6Child_frostbiteTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table6Child_frostbiteTable::Table6Child_frostbiteTable(std::vector<Table6Child_frostbiteRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table6Child_frostbiteTable::~Table6Child_frostbiteTable()
    {
    }
    
    void* Table6Child_frostbiteTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table6Child_frostbiteRow(row, ((Table6Child_frostbiteTable*)(table)));
    }
    
    const Table6Child_frostbiteRow* Table6Child_frostbiteTable::Find(char Olag) const
    {
        return this->FindRow(Olag);
    }
    
    Table6Child_interceptorRow::Table6Child_interceptorRow(CremaReader::irow& row, Table6Child_interceptorTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Antoinette = row.to_single(0);
        if (row.has_value(1))
        {
            this->Neron = ((Type36)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->Esther = row.to_boolean(2);
        }
        this->antipasto = ((Type24)(row.to_int32(3)));
        if (row.has_value(4))
        {
            this->archduchess = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->imperviousness = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->crystallographic = row.to_string(6);
        }
        this->SetKey(this->Antoinette, this->antipasto);
    }
    
    Table6Child_interceptorTable::Table6Child_interceptorTable()
    {
    }
    
    Table6Child_interceptorTable::Table6Child_interceptorTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table6Child_interceptorTable::Table6Child_interceptorTable(std::vector<Table6Child_interceptorRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table6Child_interceptorTable::~Table6Child_interceptorTable()
    {
    }
    
    void* Table6Child_interceptorTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table6Child_interceptorRow(row, ((Table6Child_interceptorTable*)(table)));
    }
    
    const Table6Child_interceptorRow* Table6Child_interceptorTable::Find(float Antoinette, Type24 antipasto) const
    {
        return this->FindRow(Antoinette, antipasto);
    }
    
    Table6Child_frostbiteTable Table6Row::Child_frostbiteEmpty;
    
    Table6Child_interceptorTable Table6Row::Child_interceptorEmpty;
    
    Table6Row::Table6Row(CremaReader::irow& row, Table6Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->reprehensible = ((Type3)(row.to_int32(0)));
        if ((this->Child_frostbite == nullptr))
        {
            this->Child_frostbite = &(Table6Row::Child_frostbiteEmpty);
        }
        if ((this->Child_interceptor == nullptr))
        {
            this->Child_interceptor = &(Table6Row::Child_interceptorEmpty);
        }
        this->SetKey(this->reprehensible);
    }
    
    void Table6SetChild_frostbite(Table6Row* target, const std::vector<Table6Child_frostbiteRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_frostbite = new Table6Child_frostbiteTable(childs);
    }
    
    void Table6SetChild_interceptor(Table6Row* target, const std::vector<Table6Child_interceptorRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_interceptor = new Table6Child_interceptorTable(childs);
    }
    
    Table6Table::Table6Table()
    {
    }
    
    Table6Table::Table6Table(CremaReader::itable& table)
    {
        this->Child_frostbite = new Table6Child_frostbiteTable(table.dataset().tables()["Table6.Child_frostbite"]);
        this->Child_interceptor = new Table6Child_interceptorTable(table.dataset().tables()["Table6.Child_interceptor"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_frostbite->Rows, Table6SetChild_frostbite);
        this->SetRelations(this->Child_interceptor->Rows, Table6SetChild_interceptor);
    }
    
    Table6Table::~Table6Table()
    {
        delete this->Child_frostbite;
        delete this->Child_interceptor;
    }
    
    void* Table6Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table6Row(row, ((Table6Table*)(table)));
    }
    
    const Table6Row* Table6Table::Find(Type3 reprehensible) const
    {
        return this->FindRow(reprehensible);
    }
    
    Table172Row::Table172Row(CremaReader::irow& row, Table172Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Chile = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->triumphant = row.to_string(1);
        }
        this->SetKey(this->Chile);
    }
    
    Table172Table::Table172Table()
    {
    }
    
    Table172Table::Table172Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table172Table::~Table172Table()
    {
    }
    
    void* Table172Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table172Row(row, ((Table172Table*)(table)));
    }
    
    const Table172Row* Table172Table::Find(unsigned long long Chile) const
    {
        return this->FindRow(Chile);
    }
    
    Table_computingRow::Table_computingRow(CremaReader::irow& row, Table_computingTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->nosh = row.to_duration(0);
        if (row.has_value(1))
        {
            this->Dee = ((Type_Arlan)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->majesty = row.to_boolean(2);
        }
        this->SetKey(this->nosh);
    }
    
    Table_computingTable::Table_computingTable()
    {
    }
    
    Table_computingTable::Table_computingTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_computingTable::~Table_computingTable()
    {
    }
    
    void* Table_computingTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_computingRow(row, ((Table_computingTable*)(table)));
    }
    
    const Table_computingRow* Table_computingTable::Find(int nosh) const
    {
        return this->FindRow(nosh);
    }
    
    Table124Row::Table124Row(CremaReader::irow& row, Table124Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->suspensive = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->kidding = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->elicit = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->dug = row.to_datetime(3);
        }
        this->SetKey(this->suspensive);
    }
    
    Table124Table::Table124Table()
    {
    }
    
    Table124Table::Table124Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table124Table::~Table124Table()
    {
    }
    
    void* Table124Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table124Row(row, ((Table124Table*)(table)));
    }
    
    const Table124Row* Table124Table::Find(unsigned char suspensive) const
    {
        return this->FindRow(suspensive);
    }
    
    Table106Row::Table106Row(CremaReader::irow& row, Table106Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->neighborliness = row.to_int64(0);
        if (row.has_value(1))
        {
            this->axiomatize = row.to_string(1);
        }
        this->SetKey(this->neighborliness);
    }
    
    Table106Table::Table106Table()
    {
    }
    
    Table106Table::Table106Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table106Table::~Table106Table()
    {
    }
    
    void* Table106Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table106Row(row, ((Table106Table*)(table)));
    }
    
    const Table106Row* Table106Table::Find(long long neighborliness) const
    {
        return this->FindRow(neighborliness);
    }
    
    Table23Row::Table23Row(CremaReader::irow& row, Table23Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Giff = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Cardenas = row.to_string(1);
        }
        this->quizzing = row.to_datetime(2);
        if (row.has_value(3))
        {
            this->carryover = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->mangy = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->scaffolding = ((Type_HeraclitusDeletable)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Janeczka = row.to_uint8(6);
        }
        this->SetKey(this->Giff, this->quizzing);
    }
    
    Table23Table::Table23Table()
    {
    }
    
    Table23Table::Table23Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table23Table::~Table23Table()
    {
    }
    
    void* Table23Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table23Row(row, ((Table23Table*)(table)));
    }
    
    const Table23Row* Table23Table::Find(int Giff, time_t quizzing) const
    {
        return this->FindRow(Giff, quizzing);
    }
    
    Table60Row::Table60Row(CremaReader::irow& row, Table60Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->ratted = ((Type_HeraclitusDeletable)(row.to_int32(0)));
        this->recycle = row.to_double(1);
        if (row.has_value(2))
        {
            this->swill = ((Type15)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->roadwork = row.to_uint16(3);
        }
        if (row.has_value(4))
        {
            this->Platonism = row.to_uint16(4);
        }
        if (row.has_value(5))
        {
            this->seaworthy = row.to_int16(5);
        }
        if (row.has_value(6))
        {
            this->underexposure = row.to_int16(6);
        }
        if (row.has_value(7))
        {
            this->amniotic = ((Type15)(row.to_int32(7)));
        }
        this->SetKey(this->ratted, this->recycle);
    }
    
    Table60Table::Table60Table()
    {
    }
    
    Table60Table::Table60Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table60Table::~Table60Table()
    {
    }
    
    void* Table60Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table60Row(row, ((Table60Table*)(table)));
    }
    
    const Table60Row* Table60Table::Find(Type_HeraclitusDeletable ratted, double recycle) const
    {
        return this->FindRow(ratted, recycle);
    }
    
    Table_KitChild1Row::Table_KitChild1Row(CremaReader::irow& row, Table_KitChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->pill = row.to_boolean(3);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table_KitChild1Table::Table_KitChild1Table()
    {
    }
    
    Table_KitChild1Table::Table_KitChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_KitChild1Table::Table_KitChild1Table(std::vector<Table_KitChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_KitChild1Table::~Table_KitChild1Table()
    {
    }
    
    void* Table_KitChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_KitChild1Row(row, ((Table_KitChild1Table*)(table)));
    }
    
    const Table_KitChild1Row* Table_KitChild1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table_KitChild_newsprintRow::Table_KitChild_newsprintRow(CremaReader::irow& row, Table_KitChild_newsprintTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->condominium = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->guiltlessness = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->splash = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->entrapping = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->Hamnet = row.to_datetime(5);
        }
        this->SetKey(this->outgrip);
    }
    
    Table_KitChild_newsprintTable::Table_KitChild_newsprintTable()
    {
    }
    
    Table_KitChild_newsprintTable::Table_KitChild_newsprintTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_KitChild_newsprintTable::Table_KitChild_newsprintTable(std::vector<Table_KitChild_newsprintRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_KitChild_newsprintTable::~Table_KitChild_newsprintTable()
    {
    }
    
    void* Table_KitChild_newsprintTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_KitChild_newsprintRow(row, ((Table_KitChild_newsprintTable*)(table)));
    }
    
    const Table_KitChild_newsprintRow* Table_KitChild_newsprintTable::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table_KitChild_nevusRow::Table_KitChild_nevusRow(CremaReader::irow& row, Table_KitChild_nevusTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->repetition = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->phonetician = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->Nanni = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->monographs = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->humus = row.to_int64(4);
        }
        this->SetKey(this->repetition);
    }
    
    Table_KitChild_nevusTable::Table_KitChild_nevusTable()
    {
    }
    
    Table_KitChild_nevusTable::Table_KitChild_nevusTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_KitChild_nevusTable::Table_KitChild_nevusTable(std::vector<Table_KitChild_nevusRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_KitChild_nevusTable::~Table_KitChild_nevusTable()
    {
    }
    
    void* Table_KitChild_nevusTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_KitChild_nevusRow(row, ((Table_KitChild_nevusTable*)(table)));
    }
    
    const Table_KitChild_nevusRow* Table_KitChild_nevusTable::Find(unsigned char repetition) const
    {
        return this->FindRow(repetition);
    }
    
    Table_KitChild3Row::Table_KitChild3Row(CremaReader::irow& row, Table_KitChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->vaginae = row.to_int64(0);
        if (row.has_value(1))
        {
            this->obvious = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->sachet = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->underpayment = ((Type11)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->axle = row.to_int8(4);
        }
        this->SetKey(this->vaginae);
    }
    
    Table_KitChild3Table::Table_KitChild3Table()
    {
    }
    
    Table_KitChild3Table::Table_KitChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_KitChild3Table::Table_KitChild3Table(std::vector<Table_KitChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_KitChild3Table::~Table_KitChild3Table()
    {
    }
    
    void* Table_KitChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_KitChild3Row(row, ((Table_KitChild3Table*)(table)));
    }
    
    const Table_KitChild3Row* Table_KitChild3Table::Find(long long vaginae) const
    {
        return this->FindRow(vaginae);
    }
    
    Table_KitChild2Row::Table_KitChild2Row(CremaReader::irow& row, Table_KitChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Enos = row.to_single(0);
        if (row.has_value(1))
        {
            this->pervasive = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->lubricator = row.to_boolean(2);
        }
        this->hallmark = row.to_string(3);
        this->SetKey(this->Enos, this->hallmark);
    }
    
    Table_KitChild2Table::Table_KitChild2Table()
    {
    }
    
    Table_KitChild2Table::Table_KitChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_KitChild2Table::Table_KitChild2Table(std::vector<Table_KitChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_KitChild2Table::~Table_KitChild2Table()
    {
    }
    
    void* Table_KitChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_KitChild2Row(row, ((Table_KitChild2Table*)(table)));
    }
    
    const Table_KitChild2Row* Table_KitChild2Table::Find(float Enos, const std::string& hallmark) const
    {
        return this->FindRow(Enos, hallmark);
    }
    
    Table_KitChild1Table Table_KitRow::Child1Empty;
    
    Table_KitChild_newsprintTable Table_KitRow::Child_newsprintEmpty;
    
    Table_KitChild_nevusTable Table_KitRow::Child_nevusEmpty;
    
    Table_KitChild3Table Table_KitRow::Child3Empty;
    
    Table_KitChild2Table Table_KitRow::Child2Empty;
    
    Table_KitRow::Table_KitRow(CremaReader::irow& row, Table_KitTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->operetta = row.to_int64(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_KitRow::Child1Empty);
        }
        if ((this->Child_newsprint == nullptr))
        {
            this->Child_newsprint = &(Table_KitRow::Child_newsprintEmpty);
        }
        if ((this->Child_nevus == nullptr))
        {
            this->Child_nevus = &(Table_KitRow::Child_nevusEmpty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_KitRow::Child3Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_KitRow::Child2Empty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table_KitSetChild1(Table_KitRow* target, const std::vector<Table_KitChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_KitChild1Table(childs);
    }
    
    void Table_KitSetChild_newsprint(Table_KitRow* target, const std::vector<Table_KitChild_newsprintRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_newsprint = new Table_KitChild_newsprintTable(childs);
    }
    
    void Table_KitSetChild_nevus(Table_KitRow* target, const std::vector<Table_KitChild_nevusRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_nevus = new Table_KitChild_nevusTable(childs);
    }
    
    void Table_KitSetChild3(Table_KitRow* target, const std::vector<Table_KitChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_KitChild3Table(childs);
    }
    
    void Table_KitSetChild2(Table_KitRow* target, const std::vector<Table_KitChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_KitChild2Table(childs);
    }
    
    Table_KitTable::Table_KitTable()
    {
    }
    
    Table_KitTable::Table_KitTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_KitChild1Table(table.dataset().tables()["Table_Kit.Child1"]);
        this->Child_newsprint = new Table_KitChild_newsprintTable(table.dataset().tables()["Table_Kit.Child_newsprint"]);
        this->Child_nevus = new Table_KitChild_nevusTable(table.dataset().tables()["Table_Kit.Child_nevus"]);
        this->Child3 = new Table_KitChild3Table(table.dataset().tables()["Table_Kit.Child3"]);
        this->Child2 = new Table_KitChild2Table(table.dataset().tables()["Table_Kit.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_KitSetChild1);
        this->SetRelations(this->Child_newsprint->Rows, Table_KitSetChild_newsprint);
        this->SetRelations(this->Child_nevus->Rows, Table_KitSetChild_nevus);
        this->SetRelations(this->Child3->Rows, Table_KitSetChild3);
        this->SetRelations(this->Child2->Rows, Table_KitSetChild2);
    }
    
    Table_KitTable::~Table_KitTable()
    {
        delete this->Child1;
        delete this->Child_newsprint;
        delete this->Child_nevus;
        delete this->Child3;
        delete this->Child2;
    }
    
    void* Table_KitTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_KitRow(row, ((Table_KitTable*)(table)));
    }
    
    const Table_KitRow* Table_KitTable::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table189Row::Table189Row(CremaReader::irow& row, Table189Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->spiritualist = ((Type_salesmen)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Atari = ((Type_guttering)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->WI = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->undiscriminating = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->archaist = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->cubic = row.to_datetime(5);
        }
        if (row.has_value(6))
        {
            this->Roxanna = row.to_datetime(6);
        }
        if (row.has_value(7))
        {
            this->emeritus = row.to_boolean(7);
        }
        this->SetKey(this->spiritualist);
    }
    
    Table189Table::Table189Table()
    {
    }
    
    Table189Table::Table189Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table189Table::~Table189Table()
    {
    }
    
    void* Table189Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table189Row(row, ((Table189Table*)(table)));
    }
    
    const Table189Row* Table189Table::Find(Type_salesmen spiritualist) const
    {
        return this->FindRow(spiritualist);
    }
    
    Table105Child_fatalisticallyRow::Table105Child_fatalisticallyRow(CremaReader::irow& row, Table105Child_fatalisticallyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->relater = row.to_int8(0);
        if (row.has_value(1))
        {
            this->cabinetry = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->prioritize = row.to_uint16(2);
        }
        if (row.has_value(3))
        {
            this->pugilist = row.to_duration(3);
        }
        this->SetKey(this->relater);
    }
    
    Table105Child_fatalisticallyTable::Table105Child_fatalisticallyTable()
    {
    }
    
    Table105Child_fatalisticallyTable::Table105Child_fatalisticallyTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table105Child_fatalisticallyTable::Table105Child_fatalisticallyTable(std::vector<Table105Child_fatalisticallyRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table105Child_fatalisticallyTable::~Table105Child_fatalisticallyTable()
    {
    }
    
    void* Table105Child_fatalisticallyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table105Child_fatalisticallyRow(row, ((Table105Child_fatalisticallyTable*)(table)));
    }
    
    const Table105Child_fatalisticallyRow* Table105Child_fatalisticallyTable::Find(char relater) const
    {
        return this->FindRow(relater);
    }
    
    Table105Child_fatalisticallyTable Table105Row::Child_fatalisticallyEmpty;
    
    Table105Row::Table105Row(CremaReader::irow& row, Table105Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Killebrew = row.to_single(0);
        if (row.has_value(1))
        {
            this->guillotine = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->partake = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->Rafael = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->lated = row.to_uint16(4);
        }
        if ((this->Child_fatalistically == nullptr))
        {
            this->Child_fatalistically = &(Table105Row::Child_fatalisticallyEmpty);
        }
        this->SetKey(this->Killebrew);
    }
    
    void Table105SetChild_fatalistically(Table105Row* target, const std::vector<Table105Child_fatalisticallyRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_fatalistically = new Table105Child_fatalisticallyTable(childs);
    }
    
    Table105Table::Table105Table()
    {
    }
    
    Table105Table::Table105Table(CremaReader::itable& table)
    {
        this->Child_fatalistically = new Table105Child_fatalisticallyTable(table.dataset().tables()["Table105.Child_fatalistically"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_fatalistically->Rows, Table105SetChild_fatalistically);
    }
    
    Table105Table::~Table105Table()
    {
        delete this->Child_fatalistically;
    }
    
    void* Table105Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table105Row(row, ((Table105Table*)(table)));
    }
    
    const Table105Row* Table105Table::Find(float Killebrew) const
    {
        return this->FindRow(Killebrew);
    }
    
    Table15Row::Table15Row(CremaReader::irow& row, Table15Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->snapback = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->caseworker = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Adonis = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->borderer = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->rattling = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->South = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->wagging = row.to_uint32(6);
        }
        if (row.has_value(7))
        {
            this->deleterious = row.to_duration(7);
        }
        if (row.has_value(8))
        {
            this->Dalmatian = row.to_int64(8);
        }
        if (row.has_value(9))
        {
            this->sprang = row.to_double(9);
        }
        if (row.has_value(10))
        {
            this->scruffy = row.to_single(10);
        }
        this->SetKey(this->snapback);
    }
    
    Table15Table::Table15Table()
    {
    }
    
    Table15Table::Table15Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table15Table::~Table15Table()
    {
    }
    
    void* Table15Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table15Row(row, ((Table15Table*)(table)));
    }
    
    const Table15Row* Table15Table::Find(unsigned int snapback) const
    {
        return this->FindRow(snapback);
    }
    
    Table16Child1Row::Table16Child1Row(CremaReader::irow& row, Table16Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->showbiz = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->beadle = row.to_boolean(1);
        }
        this->SetKey(this->showbiz);
    }
    
    Table16Child1Table::Table16Child1Table()
    {
    }
    
    Table16Child1Table::Table16Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table16Child1Table::Table16Child1Table(std::vector<Table16Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table16Child1Table::~Table16Child1Table()
    {
    }
    
    void* Table16Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table16Child1Row(row, ((Table16Child1Table*)(table)));
    }
    
    const Table16Child1Row* Table16Child1Table::Find(unsigned long long showbiz) const
    {
        return this->FindRow(showbiz);
    }
    
    Table16Child2Row::Table16Child2Row(CremaReader::irow& row, Table16Child2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->contrapositive = row.to_int32(0);
        if (row.has_value(1))
        {
            this->chelation = ((Type_hyperboloidal)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->Candlewick = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->hex = ((Type36)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->sanctimony = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->verminous = ((Type50)(row.to_int32(5)));
        }
        this->SetKey(this->contrapositive);
    }
    
    Table16Child2Table::Table16Child2Table()
    {
    }
    
    Table16Child2Table::Table16Child2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table16Child2Table::Table16Child2Table(std::vector<Table16Child2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table16Child2Table::~Table16Child2Table()
    {
    }
    
    void* Table16Child2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table16Child2Row(row, ((Table16Child2Table*)(table)));
    }
    
    const Table16Child2Row* Table16Child2Table::Find(int contrapositive) const
    {
        return this->FindRow(contrapositive);
    }
    
    Table16Child1Table Table16Row::Child1Empty;
    
    Table16Child2Table Table16Row::Child2Empty;
    
    Table16Row::Table16Row(CremaReader::irow& row, Table16Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Waugh = row.to_double(0);
        if (row.has_value(1))
        {
            this->livery = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->Bruxelles = row.to_double(2);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table16Row::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table16Row::Child2Empty);
        }
        this->SetKey(this->Waugh);
    }
    
    void Table16SetChild1(Table16Row* target, const std::vector<Table16Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table16Child1Table(childs);
    }
    
    void Table16SetChild2(Table16Row* target, const std::vector<Table16Child2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table16Child2Table(childs);
    }
    
    Table16Table::Table16Table()
    {
    }
    
    Table16Table::Table16Table(CremaReader::itable& table)
    {
        this->Child1 = new Table16Child1Table(table.dataset().tables()["Table16.Child1"]);
        this->Child2 = new Table16Child2Table(table.dataset().tables()["Table16.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table16SetChild1);
        this->SetRelations(this->Child2->Rows, Table16SetChild2);
    }
    
    Table16Table::~Table16Table()
    {
        delete this->Child1;
        delete this->Child2;
    }
    
    void* Table16Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table16Row(row, ((Table16Table*)(table)));
    }
    
    const Table16Row* Table16Table::Find(double Waugh) const
    {
        return this->FindRow(Waugh);
    }
    
    Table32Row::Table32Row(CremaReader::irow& row, Table32Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Darren = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->treasonous = ((Type_occlusionDeletable)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->overseen = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->eclogue = row.to_uint32(3);
        }
        this->SetKey(this->Darren);
    }
    
    Table32Table::Table32Table()
    {
    }
    
    Table32Table::Table32Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table32Table::~Table32Table()
    {
    }
    
    void* Table32Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table32Row(row, ((Table32Table*)(table)));
    }
    
    const Table32Row* Table32Table::Find(unsigned int Darren) const
    {
        return this->FindRow(Darren);
    }
    
    Table4Child1Row::Table4Child1Row(CremaReader::irow& row, Table4Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->woodlouse = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Letitia = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->Livonia = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->Praia = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->Christmas = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->earner = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->forwent = row.to_duration(6);
        }
        if (row.has_value(7))
        {
            this->dependability = row.to_uint32(7);
        }
        this->SetKey(this->woodlouse);
    }
    
    Table4Child1Table::Table4Child1Table()
    {
    }
    
    Table4Child1Table::Table4Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table4Child1Table::Table4Child1Table(std::vector<Table4Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table4Child1Table::~Table4Child1Table()
    {
    }
    
    void* Table4Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table4Child1Row(row, ((Table4Child1Table*)(table)));
    }
    
    const Table4Child1Row* Table4Child1Table::Find(unsigned short woodlouse) const
    {
        return this->FindRow(woodlouse);
    }
    
    Table4Child2Row::Table4Child2Row(CremaReader::irow& row, Table4Child2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Lynn = row.to_int8(0);
        if (row.has_value(1))
        {
            this->clause = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->scrupulosity = row.to_int16(2);
        }
        this->abbrev = row.to_uint16(3);
        if (row.has_value(4))
        {
            this->microdot = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->Estes = ((Type_Multan)(row.to_int32(5)));
        }
        this->SetKey(this->Lynn, this->abbrev);
    }
    
    Table4Child2Table::Table4Child2Table()
    {
    }
    
    Table4Child2Table::Table4Child2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table4Child2Table::Table4Child2Table(std::vector<Table4Child2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table4Child2Table::~Table4Child2Table()
    {
    }
    
    void* Table4Child2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table4Child2Row(row, ((Table4Child2Table*)(table)));
    }
    
    const Table4Child2Row* Table4Child2Table::Find(char Lynn, unsigned short abbrev) const
    {
        return this->FindRow(Lynn, abbrev);
    }
    
    Table4Child_tetrameterRow::Table4Child_tetrameterRow(CremaReader::irow& row, Table4Child_tetrameterTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->illegality = row.to_uint8(0);
        this->metricize = row.to_uint32(1);
        if (row.has_value(2))
        {
            this->bong = row.to_string(2);
        }
        this->confound = row.to_uint32(3);
        if (row.has_value(4))
        {
            this->coprophagous = row.to_boolean(4);
        }
        this->rosin = row.to_uint32(5);
        this->SetKey(this->illegality, this->metricize, this->confound, this->rosin);
    }
    
    Table4Child_tetrameterTable::Table4Child_tetrameterTable()
    {
    }
    
    Table4Child_tetrameterTable::Table4Child_tetrameterTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table4Child_tetrameterTable::Table4Child_tetrameterTable(std::vector<Table4Child_tetrameterRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table4Child_tetrameterTable::~Table4Child_tetrameterTable()
    {
    }
    
    void* Table4Child_tetrameterTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table4Child_tetrameterRow(row, ((Table4Child_tetrameterTable*)(table)));
    }
    
    const Table4Child_tetrameterRow* Table4Child_tetrameterTable::Find(unsigned char illegality, unsigned int metricize, unsigned int confound, unsigned int rosin) const
    {
        return this->FindRow(illegality, metricize, confound, rosin);
    }
    
    Table4Child1Table Table4Row::Child1Empty;
    
    Table4Child2Table Table4Row::Child2Empty;
    
    Table4Child_tetrameterTable Table4Row::Child_tetrameterEmpty;
    
    Table4Row::Table4Row(CremaReader::irow& row, Table4Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->multiplicative = ((Type_Multan)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->math = ((Type_Meiji)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->rickshaw = ((Type_Meiji)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->dimmed = ((Type_Arlan)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->Prussia = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->copied = ((Type_Arlan)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->pinwheel = row.to_uint64(6);
        }
        if (row.has_value(7))
        {
            this->vulture = row.to_boolean(7);
        }
        if (row.has_value(8))
        {
            this->unaccommodating = row.to_single(8);
        }
        if (row.has_value(9))
        {
            this->subprocess = row.to_int16(9);
        }
        if (row.has_value(10))
        {
            this->spadeful = row.to_uint32(10);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table4Row::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table4Row::Child2Empty);
        }
        if ((this->Child_tetrameter == nullptr))
        {
            this->Child_tetrameter = &(Table4Row::Child_tetrameterEmpty);
        }
        this->SetKey(this->multiplicative);
    }
    
    void Table4SetChild1(Table4Row* target, const std::vector<Table4Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table4Child1Table(childs);
    }
    
    void Table4SetChild2(Table4Row* target, const std::vector<Table4Child2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table4Child2Table(childs);
    }
    
    void Table4SetChild_tetrameter(Table4Row* target, const std::vector<Table4Child_tetrameterRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_tetrameter = new Table4Child_tetrameterTable(childs);
    }
    
    Table4Table::Table4Table()
    {
    }
    
    Table4Table::Table4Table(CremaReader::itable& table)
    {
        this->Child1 = new Table4Child1Table(table.dataset().tables()["Table4.Child1"]);
        this->Child2 = new Table4Child2Table(table.dataset().tables()["Table4.Child2"]);
        this->Child_tetrameter = new Table4Child_tetrameterTable(table.dataset().tables()["Table4.Child_tetrameter"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table4SetChild1);
        this->SetRelations(this->Child2->Rows, Table4SetChild2);
        this->SetRelations(this->Child_tetrameter->Rows, Table4SetChild_tetrameter);
    }
    
    Table4Table::~Table4Table()
    {
        delete this->Child1;
        delete this->Child2;
        delete this->Child_tetrameter;
    }
    
    void* Table4Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table4Row(row, ((Table4Table*)(table)));
    }
    
    const Table4Row* Table4Table::Find(Type_Multan multiplicative) const
    {
        return this->FindRow(multiplicative);
    }
    
    Table48Row::Table48Row(CremaReader::irow& row, Table48Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Wendeline = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->whiten = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Romans = ((Type25)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Appalachian = ((Type13)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->Denmark = row.to_uint16(4);
        }
        if (row.has_value(5))
        {
            this->Merrie = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->premeditated = row.to_int16(6);
        }
        if (row.has_value(7))
        {
            this->Berzelius = ((Type_rennet)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->Angelia = ((Type_Attn)(row.to_int32(8)));
        }
        this->SetKey(this->Wendeline);
    }
    
    Table48Table::Table48Table()
    {
    }
    
    Table48Table::Table48Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table48Table::~Table48Table()
    {
    }
    
    void* Table48Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table48Row(row, ((Table48Table*)(table)));
    }
    
    const Table48Row* Table48Table::Find(unsigned char Wendeline) const
    {
        return this->FindRow(Wendeline);
    }
    
    Table_apocryphalnessRow::Table_apocryphalnessRow(CremaReader::irow& row, Table_apocryphalnessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Portsmouth = row.to_int16(0);
        if (row.has_value(1))
        {
            this->bout = ((Type_rennet)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->Maimonides = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->paved = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->exaggeration = row.to_int32(4);
        }
        this->SetKey(this->Portsmouth);
    }
    
    Table_apocryphalnessTable::Table_apocryphalnessTable()
    {
    }
    
    Table_apocryphalnessTable::Table_apocryphalnessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_apocryphalnessTable::~Table_apocryphalnessTable()
    {
    }
    
    void* Table_apocryphalnessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_apocryphalnessRow(row, ((Table_apocryphalnessTable*)(table)));
    }
    
    const Table_apocryphalnessRow* Table_apocryphalnessTable::Find(short Portsmouth) const
    {
        return this->FindRow(Portsmouth);
    }
    
    Table_intactnessRow::Table_intactnessRow(CremaReader::irow& row, Table_intactnessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->collocate = row.to_int64(0);
        if (row.has_value(1))
        {
            this->invigoration = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->wetback = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->bespectacled = row.to_string(3);
        }
        this->SetKey(this->collocate);
    }
    
    Table_intactnessTable::Table_intactnessTable()
    {
    }
    
    Table_intactnessTable::Table_intactnessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_intactnessTable::~Table_intactnessTable()
    {
    }
    
    void* Table_intactnessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_intactnessRow(row, ((Table_intactnessTable*)(table)));
    }
    
    const Table_intactnessRow* Table_intactnessTable::Find(long long collocate) const
    {
        return this->FindRow(collocate);
    }
    
    Table_CaribbeanChild1Row::Table_CaribbeanChild1Row(CremaReader::irow& row, Table_CaribbeanChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->fretting = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->metallic = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->Valaree = row.to_string(2);
        }
        this->SetKey(this->fretting);
    }
    
    Table_CaribbeanChild1Table::Table_CaribbeanChild1Table()
    {
    }
    
    Table_CaribbeanChild1Table::Table_CaribbeanChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_CaribbeanChild1Table::Table_CaribbeanChild1Table(std::vector<Table_CaribbeanChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_CaribbeanChild1Table::~Table_CaribbeanChild1Table()
    {
    }
    
    void* Table_CaribbeanChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_CaribbeanChild1Row(row, ((Table_CaribbeanChild1Table*)(table)));
    }
    
    const Table_CaribbeanChild1Row* Table_CaribbeanChild1Table::Find(unsigned short fretting) const
    {
        return this->FindRow(fretting);
    }
    
    Table_CaribbeanChild1Table Table_CaribbeanRow::Child1Empty;
    
    Table_CaribbeanRow::Table_CaribbeanRow(CremaReader::irow& row, Table_CaribbeanTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->travelogue = row.to_single(0);
        if (row.has_value(1))
        {
            this->divider = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->schmuck = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->epistemic = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->Gaulish = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->apportionment = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->toxin = ((Type_Arlan)(row.to_int32(6)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_CaribbeanRow::Child1Empty);
        }
        this->SetKey(this->travelogue);
    }
    
    void Table_CaribbeanSetChild1(Table_CaribbeanRow* target, const std::vector<Table_CaribbeanChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_CaribbeanChild1Table(childs);
    }
    
    Table_CaribbeanTable::Table_CaribbeanTable()
    {
    }
    
    Table_CaribbeanTable::Table_CaribbeanTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_CaribbeanChild1Table(table.dataset().tables()["Table_Caribbean.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_CaribbeanSetChild1);
    }
    
    Table_CaribbeanTable::~Table_CaribbeanTable()
    {
        delete this->Child1;
    }
    
    void* Table_CaribbeanTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_CaribbeanRow(row, ((Table_CaribbeanTable*)(table)));
    }
    
    const Table_CaribbeanRow* Table_CaribbeanTable::Find(float travelogue) const
    {
        return this->FindRow(travelogue);
    }
    
    Table_crupperRow::Table_crupperRow(CremaReader::irow& row, Table_crupperTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Wendeline = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->whiten = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Romans = ((Type25)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Denmark = row.to_uint16(3);
        }
        if (row.has_value(4))
        {
            this->Merrie = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->premeditated = row.to_int16(5);
        }
        if (row.has_value(6))
        {
            this->Berzelius = ((Type_rennet)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->Angelia = ((Type_Attn)(row.to_int32(7)));
        }
        this->SetKey(this->Wendeline);
    }
    
    Table_crupperTable::Table_crupperTable()
    {
    }
    
    Table_crupperTable::Table_crupperTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_crupperTable::~Table_crupperTable()
    {
    }
    
    void* Table_crupperTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_crupperRow(row, ((Table_crupperTable*)(table)));
    }
    
    const Table_crupperRow* Table_crupperTable::Find(unsigned char Wendeline) const
    {
        return this->FindRow(Wendeline);
    }
    
    Table101Row::Table101Row(CremaReader::irow& row, Table101Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->seemliness = row.to_single(0);
        if (row.has_value(1))
        {
            this->sandpit = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->carbonization = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->whacker = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->happing = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->considerable = row.to_int64(5);
        }
        if (row.has_value(6))
        {
            this->mis = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->impairer = row.to_string(7);
        }
        this->SetKey(this->seemliness);
    }
    
    Table101Table::Table101Table()
    {
    }
    
    Table101Table::Table101Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table101Table::~Table101Table()
    {
    }
    
    void* Table101Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table101Row(row, ((Table101Table*)(table)));
    }
    
    const Table101Row* Table101Table::Find(float seemliness) const
    {
        return this->FindRow(seemliness);
    }
    
    Table14Row::Table14Row(CremaReader::irow& row, Table14Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->argumentation = row.to_single(0);
        if (row.has_value(1))
        {
            this->overwrite = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->line = row.to_string(2);
        }
        this->SetKey(this->argumentation);
    }
    
    Table14Table::Table14Table()
    {
    }
    
    Table14Table::Table14Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table14Table::~Table14Table()
    {
    }
    
    void* Table14Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table14Row(row, ((Table14Table*)(table)));
    }
    
    const Table14Row* Table14Table::Find(float argumentation) const
    {
        return this->FindRow(argumentation);
    }
    
    Table2Child_defisRow::Table2Child_defisRow(CremaReader::irow& row, Table2Child_defisTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->ppm = ((Type_Arlan)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Jaquenetta = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->multilingual = row.to_string(2);
        }
        this->SetKey(this->ppm);
    }
    
    Table2Child_defisTable::Table2Child_defisTable()
    {
    }
    
    Table2Child_defisTable::Table2Child_defisTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table2Child_defisTable::Table2Child_defisTable(std::vector<Table2Child_defisRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table2Child_defisTable::~Table2Child_defisTable()
    {
    }
    
    void* Table2Child_defisTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table2Child_defisRow(row, ((Table2Child_defisTable*)(table)));
    }
    
    const Table2Child_defisRow* Table2Child_defisTable::Find(Type_Arlan ppm) const
    {
        return this->FindRow(ppm);
    }
    
    Table2Child2Row::Table2Child2Row(CremaReader::irow& row, Table2Child2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->transact = row.to_duration(0);
        if (row.has_value(1))
        {
            this->stormtroopers = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->superdense = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Noll = ((Type44)(row.to_int32(3)));
        }
        this->consolidate = ((Type_situational)(row.to_int32(4)));
        if (row.has_value(5))
        {
            this->bans = row.to_int8(5);
        }
        this->SetKey(this->transact, this->consolidate);
    }
    
    Table2Child2Table::Table2Child2Table()
    {
    }
    
    Table2Child2Table::Table2Child2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table2Child2Table::Table2Child2Table(std::vector<Table2Child2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table2Child2Table::~Table2Child2Table()
    {
    }
    
    void* Table2Child2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table2Child2Row(row, ((Table2Child2Table*)(table)));
    }
    
    const Table2Child2Row* Table2Child2Table::Find(int transact, Type_situational consolidate) const
    {
        return this->FindRow(transact, consolidate);
    }
    
    Table2Child_defisTable Table2Row::Child_defisEmpty;
    
    Table2Child2Table Table2Row::Child2Empty;
    
    Table2Row::Table2Row(CremaReader::irow& row, Table2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->mayo = row.to_int64(0);
        if (row.has_value(1))
        {
            this->blaster = row.to_duration(1);
        }
        if ((this->Child_defis == nullptr))
        {
            this->Child_defis = &(Table2Row::Child_defisEmpty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table2Row::Child2Empty);
        }
        this->SetKey(this->mayo);
    }
    
    void Table2SetChild_defis(Table2Row* target, const std::vector<Table2Child_defisRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_defis = new Table2Child_defisTable(childs);
    }
    
    void Table2SetChild2(Table2Row* target, const std::vector<Table2Child2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table2Child2Table(childs);
    }
    
    Table2Table::Table2Table()
    {
    }
    
    Table2Table::Table2Table(CremaReader::itable& table)
    {
        this->Child_defis = new Table2Child_defisTable(table.dataset().tables()["Table2.Child_defis"]);
        this->Child2 = new Table2Child2Table(table.dataset().tables()["Table2.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_defis->Rows, Table2SetChild_defis);
        this->SetRelations(this->Child2->Rows, Table2SetChild2);
    }
    
    Table2Table::~Table2Table()
    {
        delete this->Child_defis;
        delete this->Child2;
    }
    
    void* Table2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table2Row(row, ((Table2Table*)(table)));
    }
    
    const Table2Row* Table2Table::Find(long long mayo) const
    {
        return this->FindRow(mayo);
    }
    
    Table29Child1Row::Table29Child1Row(CremaReader::irow& row, Table29Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->cesspool = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->vocalization = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->overbook = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->affidavit = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->circuit = row.to_boolean(4);
        }
        this->snowily = row.to_duration(5);
        this->goatherd = row.to_int32(6);
        if (row.has_value(7))
        {
            this->Moravia = ((Type_livingness)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->Lakehurst = row.to_boolean(8);
        }
        if (row.has_value(9))
        {
            this->ricer = row.to_single(9);
        }
        if (row.has_value(10))
        {
            this->sweatiness = row.to_uint64(10);
        }
        if (row.has_value(11))
        {
            this->Lugosi = row.to_uint8(11);
        }
        this->SetKey(this->cesspool, this->snowily, this->goatherd);
    }
    
    Table29Child1Table::Table29Child1Table()
    {
    }
    
    Table29Child1Table::Table29Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table29Child1Table::Table29Child1Table(std::vector<Table29Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table29Child1Table::~Table29Child1Table()
    {
    }
    
    void* Table29Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table29Child1Row(row, ((Table29Child1Table*)(table)));
    }
    
    const Table29Child1Row* Table29Child1Table::Find(unsigned long long cesspool, int snowily, int goatherd) const
    {
        return this->FindRow(cesspool, snowily, goatherd);
    }
    
    Table29Child1Table Table29Row::Child1Empty;
    
    Table29Row::Table29Row(CremaReader::irow& row, Table29Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Elicia = row.to_int8(0);
        if (row.has_value(1))
        {
            this->execrably = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->Knudsen = ((Type_rennet)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->bulky = ((Type15)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->fake = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->scintillation = row.to_single(5);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table29Row::Child1Empty);
        }
        this->SetKey(this->Elicia);
    }
    
    void Table29SetChild1(Table29Row* target, const std::vector<Table29Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table29Child1Table(childs);
    }
    
    Table29Table::Table29Table()
    {
    }
    
    Table29Table::Table29Table(CremaReader::itable& table)
    {
        this->Child1 = new Table29Child1Table(table.dataset().tables()["Table29.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table29SetChild1);
    }
    
    Table29Table::~Table29Table()
    {
        delete this->Child1;
    }
    
    void* Table29Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table29Row(row, ((Table29Table*)(table)));
    }
    
    const Table29Row* Table29Table::Find(char Elicia) const
    {
        return this->FindRow(Elicia);
    }
    
    Table36Row::Table36Row(CremaReader::irow& row, Table36Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->mongoose = row.to_double(0);
        this->paranoia = ((Type_Madison)(row.to_int32(1)));
        if (row.has_value(2))
        {
            this->rehabilitate = ((Type8)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->composition = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->equality = row.to_int16(4);
        }
        this->SetKey(this->mongoose, this->paranoia);
    }
    
    Table36Table::Table36Table()
    {
    }
    
    Table36Table::Table36Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table36Table::~Table36Table()
    {
    }
    
    void* Table36Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table36Row(row, ((Table36Table*)(table)));
    }
    
    const Table36Row* Table36Table::Find(double mongoose, Type_Madison paranoia) const
    {
        return this->FindRow(mongoose, paranoia);
    }
    
    Table79Row::Table79Row(CremaReader::irow& row, Table79Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->constitution = row.to_datetime(0);
        this->SetKey(this->constitution);
    }
    
    Table79Table::Table79Table()
    {
    }
    
    Table79Table::Table79Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table79Table::~Table79Table()
    {
    }
    
    void* Table79Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table79Row(row, ((Table79Table*)(table)));
    }
    
    const Table79Row* Table79Table::Find(time_t constitution) const
    {
        return this->FindRow(constitution);
    }
    
    Table_BriticismChild1Row::Table_BriticismChild1Row(CremaReader::irow& row, Table_BriticismChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->woodlouse = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Letitia = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->Livonia = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->Praia = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->Christmas = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->earner = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->forwent = row.to_duration(6);
        }
        if (row.has_value(7))
        {
            this->dependability = row.to_uint32(7);
        }
        this->SetKey(this->woodlouse);
    }
    
    Table_BriticismChild1Table::Table_BriticismChild1Table()
    {
    }
    
    Table_BriticismChild1Table::Table_BriticismChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_BriticismChild1Table::Table_BriticismChild1Table(std::vector<Table_BriticismChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_BriticismChild1Table::~Table_BriticismChild1Table()
    {
    }
    
    void* Table_BriticismChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_BriticismChild1Row(row, ((Table_BriticismChild1Table*)(table)));
    }
    
    const Table_BriticismChild1Row* Table_BriticismChild1Table::Find(unsigned short woodlouse) const
    {
        return this->FindRow(woodlouse);
    }
    
    Table_BriticismChild2Row::Table_BriticismChild2Row(CremaReader::irow& row, Table_BriticismChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Lynn = row.to_int8(0);
        if (row.has_value(1))
        {
            this->clause = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->scrupulosity = row.to_int16(2);
        }
        this->abbrev = row.to_uint16(3);
        if (row.has_value(4))
        {
            this->microdot = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->Estes = ((Type_Multan)(row.to_int32(5)));
        }
        this->SetKey(this->Lynn, this->abbrev);
    }
    
    Table_BriticismChild2Table::Table_BriticismChild2Table()
    {
    }
    
    Table_BriticismChild2Table::Table_BriticismChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_BriticismChild2Table::Table_BriticismChild2Table(std::vector<Table_BriticismChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_BriticismChild2Table::~Table_BriticismChild2Table()
    {
    }
    
    void* Table_BriticismChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_BriticismChild2Row(row, ((Table_BriticismChild2Table*)(table)));
    }
    
    const Table_BriticismChild2Row* Table_BriticismChild2Table::Find(char Lynn, unsigned short abbrev) const
    {
        return this->FindRow(Lynn, abbrev);
    }
    
    Table_BriticismChild_tapiocaRow::Table_BriticismChild_tapiocaRow(CremaReader::irow& row, Table_BriticismChild_tapiocaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->illegality = row.to_uint8(0);
        this->metricize = row.to_uint32(1);
        if (row.has_value(2))
        {
            this->bong = row.to_string(2);
        }
        this->confound = row.to_uint32(3);
        if (row.has_value(4))
        {
            this->coprophagous = row.to_boolean(4);
        }
        this->rosin = row.to_uint32(5);
        this->SetKey(this->illegality, this->metricize, this->confound, this->rosin);
    }
    
    Table_BriticismChild_tapiocaTable::Table_BriticismChild_tapiocaTable()
    {
    }
    
    Table_BriticismChild_tapiocaTable::Table_BriticismChild_tapiocaTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_BriticismChild_tapiocaTable::Table_BriticismChild_tapiocaTable(std::vector<Table_BriticismChild_tapiocaRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_BriticismChild_tapiocaTable::~Table_BriticismChild_tapiocaTable()
    {
    }
    
    void* Table_BriticismChild_tapiocaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_BriticismChild_tapiocaRow(row, ((Table_BriticismChild_tapiocaTable*)(table)));
    }
    
    const Table_BriticismChild_tapiocaRow* Table_BriticismChild_tapiocaTable::Find(unsigned char illegality, unsigned int metricize, unsigned int confound, unsigned int rosin) const
    {
        return this->FindRow(illegality, metricize, confound, rosin);
    }
    
    Table_BriticismChild1Table Table_BriticismRow::Child1Empty;
    
    Table_BriticismChild2Table Table_BriticismRow::Child2Empty;
    
    Table_BriticismChild_tapiocaTable Table_BriticismRow::Child_tapiocaEmpty;
    
    Table_BriticismRow::Table_BriticismRow(CremaReader::irow& row, Table_BriticismTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->multiplicative = ((Type_Multan)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->math = ((Type_Meiji)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->rickshaw = ((Type_Meiji)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->dimmed = ((Type_Arlan)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->bucker = row.to_double(4);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_BriticismRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_BriticismRow::Child2Empty);
        }
        if ((this->Child_tapioca == nullptr))
        {
            this->Child_tapioca = &(Table_BriticismRow::Child_tapiocaEmpty);
        }
        this->SetKey(this->multiplicative);
    }
    
    void Table_BriticismSetChild1(Table_BriticismRow* target, const std::vector<Table_BriticismChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_BriticismChild1Table(childs);
    }
    
    void Table_BriticismSetChild2(Table_BriticismRow* target, const std::vector<Table_BriticismChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_BriticismChild2Table(childs);
    }
    
    void Table_BriticismSetChild_tapioca(Table_BriticismRow* target, const std::vector<Table_BriticismChild_tapiocaRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_tapioca = new Table_BriticismChild_tapiocaTable(childs);
    }
    
    Table_BriticismTable::Table_BriticismTable()
    {
    }
    
    Table_BriticismTable::Table_BriticismTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_BriticismChild1Table(table.dataset().tables()["Table_Briticism.Child1"]);
        this->Child2 = new Table_BriticismChild2Table(table.dataset().tables()["Table_Briticism.Child2"]);
        this->Child_tapioca = new Table_BriticismChild_tapiocaTable(table.dataset().tables()["Table_Briticism.Child_tapioca"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_BriticismSetChild1);
        this->SetRelations(this->Child2->Rows, Table_BriticismSetChild2);
        this->SetRelations(this->Child_tapioca->Rows, Table_BriticismSetChild_tapioca);
    }
    
    Table_BriticismTable::~Table_BriticismTable()
    {
        delete this->Child1;
        delete this->Child2;
        delete this->Child_tapioca;
    }
    
    void* Table_BriticismTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_BriticismRow(row, ((Table_BriticismTable*)(table)));
    }
    
    const Table_BriticismRow* Table_BriticismTable::Find(Type_Multan multiplicative) const
    {
        return this->FindRow(multiplicative);
    }
    
    Table107Row::Table107Row(CremaReader::irow& row, Table107Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Greer = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Celestine = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->hooter = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->jackhammer = row.to_int8(3);
        }
        if (row.has_value(4))
        {
            this->lactic = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->swordsmen = ((Type8)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->hellfire = row.to_uint32(6);
        }
        if (row.has_value(7))
        {
            this->Paleolithic = row.to_uint64(7);
        }
        if (row.has_value(8))
        {
            this->crowdedness = row.to_boolean(8);
        }
        this->SetKey(this->Greer);
    }
    
    Table107Table::Table107Table()
    {
    }
    
    Table107Table::Table107Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table107Table::~Table107Table()
    {
    }
    
    void* Table107Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table107Row(row, ((Table107Table*)(table)));
    }
    
    const Table107Row* Table107Table::Find(int Greer) const
    {
        return this->FindRow(Greer);
    }
    
    Table_AntonyRow::Table_AntonyRow(CremaReader::irow& row, Table_AntonyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->culpableness = row.to_single(0);
        this->SetKey(this->culpableness);
    }
    
    Table_AntonyTable::Table_AntonyTable()
    {
    }
    
    Table_AntonyTable::Table_AntonyTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_AntonyTable::~Table_AntonyTable()
    {
    }
    
    void* Table_AntonyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_AntonyRow(row, ((Table_AntonyTable*)(table)));
    }
    
    const Table_AntonyRow* Table_AntonyTable::Find(float culpableness) const
    {
        return this->FindRow(culpableness);
    }
    
    Table140Row::Table140Row(CremaReader::irow& row, Table140Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->renouncement = row.to_single(0);
        if (row.has_value(1))
        {
            this->Gennie = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->different = row.to_int16(2);
        }
        this->unfashionable = row.to_duration(3);
        if (row.has_value(4))
        {
            this->gesticulation = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->eyeless = row.to_uint16(5);
        }
        this->SetKey(this->renouncement, this->unfashionable);
    }
    
    Table140Table::Table140Table()
    {
    }
    
    Table140Table::Table140Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table140Table::~Table140Table()
    {
    }
    
    void* Table140Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table140Row(row, ((Table140Table*)(table)));
    }
    
    const Table140Row* Table140Table::Find(float renouncement, int unfashionable) const
    {
        return this->FindRow(renouncement, unfashionable);
    }
    
    Table27Row::Table27Row(CremaReader::irow& row, Table27Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->unnavigable = row.to_single(0);
        if (row.has_value(1))
        {
            this->Jarad = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->separates = row.to_int64(2);
        }
        this->assiduity = row.to_datetime(3);
        if (row.has_value(4))
        {
            this->yardmaster = ((Type8)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->Sir = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->thermostat = ((Type_Attn)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->Gregoor = row.to_string(7);
        }
        if (row.has_value(8))
        {
            this->planetarium = row.to_uint16(8);
        }
        this->SetKey(this->unnavigable, this->assiduity);
    }
    
    Table27Table::Table27Table()
    {
    }
    
    Table27Table::Table27Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table27Table::~Table27Table()
    {
    }
    
    void* Table27Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table27Row(row, ((Table27Table*)(table)));
    }
    
    const Table27Row* Table27Table::Find(float unnavigable, time_t assiduity) const
    {
        return this->FindRow(unnavigable, assiduity);
    }
    
    Table_JuddChild_roguishnessRow::Table_JuddChild_roguishnessRow(CremaReader::irow& row, Table_JuddChild_roguishnessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->exudation = row.to_duration(0);
        this->SetKey(this->exudation);
    }
    
    Table_JuddChild_roguishnessTable::Table_JuddChild_roguishnessTable()
    {
    }
    
    Table_JuddChild_roguishnessTable::Table_JuddChild_roguishnessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_JuddChild_roguishnessTable::Table_JuddChild_roguishnessTable(std::vector<Table_JuddChild_roguishnessRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_JuddChild_roguishnessTable::~Table_JuddChild_roguishnessTable()
    {
    }
    
    void* Table_JuddChild_roguishnessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_JuddChild_roguishnessRow(row, ((Table_JuddChild_roguishnessTable*)(table)));
    }
    
    const Table_JuddChild_roguishnessRow* Table_JuddChild_roguishnessTable::Find(int exudation) const
    {
        return this->FindRow(exudation);
    }
    
    Table_JuddChild_roguishnessTable Table_JuddRow::Child_roguishnessEmpty;
    
    Table_JuddRow::Table_JuddRow(CremaReader::irow& row, Table_JuddTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->corpsman = row.to_double(0);
        if (row.has_value(1))
        {
            this->Kaposi = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->hyperemia = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->pensiveness = row.to_single(3);
        }
        this->jetting = row.to_boolean(4);
        if (row.has_value(5))
        {
            this->babe = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->clears = row.to_single(6);
        }
        if (row.has_value(7))
        {
            this->codetermine = row.to_duration(7);
        }
        if ((this->Child_roguishness == nullptr))
        {
            this->Child_roguishness = &(Table_JuddRow::Child_roguishnessEmpty);
        }
        this->SetKey(this->corpsman, this->jetting);
    }
    
    void Table_JuddSetChild_roguishness(Table_JuddRow* target, const std::vector<Table_JuddChild_roguishnessRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_roguishness = new Table_JuddChild_roguishnessTable(childs);
    }
    
    Table_JuddTable::Table_JuddTable()
    {
    }
    
    Table_JuddTable::Table_JuddTable(CremaReader::itable& table)
    {
        this->Child_roguishness = new Table_JuddChild_roguishnessTable(table.dataset().tables()["Table_Judd.Child_roguishness"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_roguishness->Rows, Table_JuddSetChild_roguishness);
    }
    
    Table_JuddTable::~Table_JuddTable()
    {
        delete this->Child_roguishness;
    }
    
    void* Table_JuddTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_JuddRow(row, ((Table_JuddTable*)(table)));
    }
    
    const Table_JuddRow* Table_JuddTable::Find(double corpsman, bool jetting) const
    {
        return this->FindRow(corpsman, jetting);
    }
    
    Table18Row::Table18Row(CremaReader::irow& row, Table18Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Flem = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->songster = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->Tenn = row.to_uint64(2);
        }
        if (row.has_value(3))
        {
            this->telepathy = ((Type_Arlan)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->withhold = row.to_int16(4);
        }
        if (row.has_value(5))
        {
            this->unoffensive = ((Type_livingness)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Piaf = ((Type_Attn)(row.to_int32(6)));
        }
        if (row.has_value(7))
        {
            this->basinful = row.to_uint32(7);
        }
        if (row.has_value(8))
        {
            this->cappuccino = row.to_int32(8);
        }
        if (row.has_value(9))
        {
            this->commemoration = row.to_uint32(9);
        }
        if (row.has_value(10))
        {
            this->blusterous = row.to_int16(10);
        }
        if (row.has_value(11))
        {
            this->shrieker = row.to_single(11);
        }
        if (row.has_value(12))
        {
            this->jacuzzi = ((Type_RhodesDeletable)(row.to_int32(12)));
        }
        if (row.has_value(13))
        {
            this->premarket = row.to_boolean(13);
        }
        this->SetKey(this->Flem);
    }
    
    Table18Table::Table18Table()
    {
    }
    
    Table18Table::Table18Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table18Table::~Table18Table()
    {
    }
    
    void* Table18Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table18Row(row, ((Table18Table*)(table)));
    }
    
    const Table18Row* Table18Table::Find(unsigned long long Flem) const
    {
        return this->FindRow(Flem);
    }
    
    Table78Row::Table78Row(CremaReader::irow& row, Table78Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Alyosha = row.to_double(0);
        if (row.has_value(1))
        {
            this->vesper = row.to_uint8(1);
        }
        this->eradicator = row.to_int16(2);
        if (row.has_value(3))
        {
            this->Tait = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->Lishe = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->Cobby = ((Type51)(row.to_int32(5)));
        }
        this->SetKey(this->Alyosha, this->eradicator);
    }
    
    Table78Table::Table78Table()
    {
    }
    
    Table78Table::Table78Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table78Table::~Table78Table()
    {
    }
    
    void* Table78Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table78Row(row, ((Table78Table*)(table)));
    }
    
    const Table78Row* Table78Table::Find(double Alyosha, short eradicator) const
    {
        return this->FindRow(Alyosha, eradicator);
    }
    
    Table_departChild2Row::Table_departChild2Row(CremaReader::irow& row, Table_departChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->irreconcilability = ((Type_consortia)(row.to_int32(0)));
        this->SetKey(this->irreconcilability);
    }
    
    Table_departChild2Table::Table_departChild2Table()
    {
    }
    
    Table_departChild2Table::Table_departChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_departChild2Table::Table_departChild2Table(std::vector<Table_departChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_departChild2Table::~Table_departChild2Table()
    {
    }
    
    void* Table_departChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_departChild2Row(row, ((Table_departChild2Table*)(table)));
    }
    
    const Table_departChild2Row* Table_departChild2Table::Find(Type_consortia irreconcilability) const
    {
        return this->FindRow(irreconcilability);
    }
    
    Table_departChild2Table Table_departRow::Child2Empty;
    
    Table_departRow::Table_departRow(CremaReader::irow& row, Table_departTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->adviser = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Peterson = ((Type_pledge)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->Henka = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->cumuli = ((Type24)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->analyzed = row.to_int32(4);
        }
        if (row.has_value(5))
        {
            this->scowler = row.to_int64(5);
        }
        if (row.has_value(6))
        {
            this->articulateness = row.to_int16(6);
        }
        if (row.has_value(7))
        {
            this->USMC = row.to_int16(7);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_departRow::Child2Empty);
        }
        this->SetKey(this->adviser);
    }
    
    void Table_departSetChild2(Table_departRow* target, const std::vector<Table_departChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_departChild2Table(childs);
    }
    
    Table_departTable::Table_departTable()
    {
    }
    
    Table_departTable::Table_departTable(CremaReader::itable& table)
    {
        this->Child2 = new Table_departChild2Table(table.dataset().tables()["Table_depart.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child2->Rows, Table_departSetChild2);
    }
    
    Table_departTable::~Table_departTable()
    {
        delete this->Child2;
    }
    
    void* Table_departTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_departRow(row, ((Table_departTable*)(table)));
    }
    
    const Table_departRow* Table_departTable::Find(unsigned short adviser) const
    {
        return this->FindRow(adviser);
    }
    
    Table_SusanneChild1Row::Table_SusanneChild1Row(CremaReader::irow& row, Table_SusanneChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->pill = row.to_boolean(3);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table_SusanneChild1Table::Table_SusanneChild1Table()
    {
    }
    
    Table_SusanneChild1Table::Table_SusanneChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_SusanneChild1Table::Table_SusanneChild1Table(std::vector<Table_SusanneChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_SusanneChild1Table::~Table_SusanneChild1Table()
    {
    }
    
    void* Table_SusanneChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_SusanneChild1Row(row, ((Table_SusanneChild1Table*)(table)));
    }
    
    const Table_SusanneChild1Row* Table_SusanneChild1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table_SusanneChild_verballedRow::Table_SusanneChild_verballedRow(CremaReader::irow& row, Table_SusanneChild_verballedTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->condominium = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->guiltlessness = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->splash = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->entrapping = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->Hamnet = row.to_datetime(5);
        }
        this->SetKey(this->outgrip);
    }
    
    Table_SusanneChild_verballedTable::Table_SusanneChild_verballedTable()
    {
    }
    
    Table_SusanneChild_verballedTable::Table_SusanneChild_verballedTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_SusanneChild_verballedTable::Table_SusanneChild_verballedTable(std::vector<Table_SusanneChild_verballedRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_SusanneChild_verballedTable::~Table_SusanneChild_verballedTable()
    {
    }
    
    void* Table_SusanneChild_verballedTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_SusanneChild_verballedRow(row, ((Table_SusanneChild_verballedTable*)(table)));
    }
    
    const Table_SusanneChild_verballedRow* Table_SusanneChild_verballedTable::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table_SusanneChild1Table Table_SusanneRow::Child1Empty;
    
    Table_SusanneChild_verballedTable Table_SusanneRow::Child_verballedEmpty;
    
    Table_SusanneRow::Table_SusanneRow(CremaReader::irow& row, Table_SusanneTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->operetta = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->overstep = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->compulsion = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->Aspell = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->immorality = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->centrifugate = row.to_uint64(8);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_SusanneRow::Child1Empty);
        }
        if ((this->Child_verballed == nullptr))
        {
            this->Child_verballed = &(Table_SusanneRow::Child_verballedEmpty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table_SusanneSetChild1(Table_SusanneRow* target, const std::vector<Table_SusanneChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_SusanneChild1Table(childs);
    }
    
    void Table_SusanneSetChild_verballed(Table_SusanneRow* target, const std::vector<Table_SusanneChild_verballedRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_verballed = new Table_SusanneChild_verballedTable(childs);
    }
    
    Table_SusanneTable::Table_SusanneTable()
    {
    }
    
    Table_SusanneTable::Table_SusanneTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_SusanneChild1Table(table.dataset().tables()["Table_Susanne.Child1"]);
        this->Child_verballed = new Table_SusanneChild_verballedTable(table.dataset().tables()["Table_Susanne.Child_verballed"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_SusanneSetChild1);
        this->SetRelations(this->Child_verballed->Rows, Table_SusanneSetChild_verballed);
    }
    
    Table_SusanneTable::~Table_SusanneTable()
    {
        delete this->Child1;
        delete this->Child_verballed;
    }
    
    void* Table_SusanneTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_SusanneRow(row, ((Table_SusanneTable*)(table)));
    }
    
    const Table_SusanneRow* Table_SusanneTable::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table126Row::Table126Row(CremaReader::irow& row, Table126Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->technicalness = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->Wilshire = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->suction = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->Sabrina = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->nightie = ((Type22)(row.to_int32(4)));
        }
        this->SetKey(this->technicalness);
    }
    
    Table126Table::Table126Table()
    {
    }
    
    Table126Table::Table126Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table126Table::~Table126Table()
    {
    }
    
    void* Table126Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table126Row(row, ((Table126Table*)(table)));
    }
    
    const Table126Row* Table126Table::Find(bool technicalness) const
    {
        return this->FindRow(technicalness);
    }
    
    Table146Row::Table146Row(CremaReader::irow& row, Table146Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->bloater = row.to_int16(0);
        if (row.has_value(1))
        {
            this->inhalant = ((Type13)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->Mike = row.to_duration(2);
        }
        this->SetKey(this->bloater);
    }
    
    Table146Table::Table146Table()
    {
    }
    
    Table146Table::Table146Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table146Table::~Table146Table()
    {
    }
    
    void* Table146Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table146Row(row, ((Table146Table*)(table)));
    }
    
    const Table146Row* Table146Table::Find(short bloater) const
    {
        return this->FindRow(bloater);
    }
    
    Table80Row::Table80Row(CremaReader::irow& row, Table80Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->narcoleptic = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->wakefulness = ((Type_canted)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->mealiness = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Miguela = ((Type_farinaceous)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->bedstead = ((Type_canted)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->hitherto = row.to_int8(5);
        }
        if (row.has_value(6))
        {
            this->Ibsen = row.to_string(6);
        }
        if (row.has_value(7))
        {
            this->Shavuot = row.to_datetime(7);
        }
        if (row.has_value(8))
        {
            this->hedgerow = row.to_datetime(8);
        }
        if (row.has_value(9))
        {
            this->video = ((Type_seasonably)(row.to_int32(9)));
        }
        if (row.has_value(10))
        {
            this->Jemmy = row.to_uint16(10);
        }
        if (row.has_value(11))
        {
            this->Kublai = row.to_uint16(11);
        }
        if (row.has_value(12))
        {
            this->newsworthiness = ((Type21)(row.to_int32(12)));
        }
        if (row.has_value(13))
        {
            this->Bowery = row.to_int32(13);
        }
        this->devour = ((Type21)(row.to_int32(14)));
        this->SetKey(this->narcoleptic, this->devour);
    }
    
    Table80Table::Table80Table()
    {
    }
    
    Table80Table::Table80Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table80Table::~Table80Table()
    {
    }
    
    void* Table80Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table80Row(row, ((Table80Table*)(table)));
    }
    
    const Table80Row* Table80Table::Find(unsigned int narcoleptic, Type21 devour) const
    {
        return this->FindRow(narcoleptic, devour);
    }
    
    Table_sclerosesRow::Table_sclerosesRow(CremaReader::irow& row, Table_sclerosesTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->choral = ((Type70)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->malting = row.to_string(1);
        }
        this->intuitive = row.to_uint8(2);
        if (row.has_value(3))
        {
            this->bandsmen = row.to_uint16(3);
        }
        this->SetKey(this->choral, this->intuitive);
    }
    
    Table_sclerosesTable::Table_sclerosesTable()
    {
    }
    
    Table_sclerosesTable::Table_sclerosesTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_sclerosesTable::~Table_sclerosesTable()
    {
    }
    
    void* Table_sclerosesTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_sclerosesRow(row, ((Table_sclerosesTable*)(table)));
    }
    
    const Table_sclerosesRow* Table_sclerosesTable::Find(Type70 choral, unsigned char intuitive) const
    {
        return this->FindRow(choral, intuitive);
    }
    
    Table177Row::Table177Row(CremaReader::irow& row, Table177Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Shillong = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->$explicit = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->angularity = row.to_int32(2);
        }
        this->SetKey(this->Shillong);
    }
    
    Table177Table::Table177Table()
    {
    }
    
    Table177Table::Table177Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table177Table::~Table177Table()
    {
    }
    
    void* Table177Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table177Row(row, ((Table177Table*)(table)));
    }
    
    const Table177Row* Table177Table::Find(unsigned short Shillong) const
    {
        return this->FindRow(Shillong);
    }
    
    Table75Row::Table75Row(CremaReader::irow& row, Table75Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->noncontroversial = row.to_int8(0);
        if (row.has_value(1))
        {
            this->conservativeness = ((Type25)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->WNW = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->rating = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->elitism = row.to_uint64(4);
        }
        this->SetKey(this->noncontroversial);
    }
    
    Table75Table::Table75Table()
    {
    }
    
    Table75Table::Table75Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table75Table::~Table75Table()
    {
    }
    
    void* Table75Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table75Row(row, ((Table75Table*)(table)));
    }
    
    const Table75Row* Table75Table::Find(char noncontroversial) const
    {
        return this->FindRow(noncontroversial);
    }
    
    Table84Child_skullcapDeletableRow::Table84Child_skullcapDeletableRow(CremaReader::irow& row, Table84Child_skullcapDeletableTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->chock = ((Type35)(row.to_int32(0)));
        this->Jemmy = row.to_duration(1);
        this->irrevocable = row.to_int8(2);
        if (row.has_value(3))
        {
            this->encumbrancer = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->Court = ((Type_Jenelle)(row.to_int32(4)));
        }
        this->SetKey(this->chock, this->Jemmy, this->irrevocable);
    }
    
    Table84Child_skullcapDeletableTable::Table84Child_skullcapDeletableTable()
    {
    }
    
    Table84Child_skullcapDeletableTable::Table84Child_skullcapDeletableTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table84Child_skullcapDeletableTable::Table84Child_skullcapDeletableTable(std::vector<Table84Child_skullcapDeletableRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table84Child_skullcapDeletableTable::~Table84Child_skullcapDeletableTable()
    {
    }
    
    void* Table84Child_skullcapDeletableTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table84Child_skullcapDeletableRow(row, ((Table84Child_skullcapDeletableTable*)(table)));
    }
    
    const Table84Child_skullcapDeletableRow* Table84Child_skullcapDeletableTable::Find(Type35 chock, int Jemmy, char irrevocable) const
    {
        return this->FindRow(chock, Jemmy, irrevocable);
    }
    
    Table84Child_skullcapDeletableTable Table84Row::Child_skullcapDeletableEmpty;
    
    Table84Row::Table84Row(CremaReader::irow& row, Table84Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Eduino = row.to_int8(0);
        if (row.has_value(1))
        {
            this->furrow = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->Foch = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->chrysalis = ((Type12)(row.to_int32(3)));
        }
        if ((this->Child_skullcapDeletable == nullptr))
        {
            this->Child_skullcapDeletable = &(Table84Row::Child_skullcapDeletableEmpty);
        }
        this->SetKey(this->Eduino);
    }
    
    void Table84SetChild_skullcapDeletable(Table84Row* target, const std::vector<Table84Child_skullcapDeletableRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_skullcapDeletable = new Table84Child_skullcapDeletableTable(childs);
    }
    
    Table84Table::Table84Table()
    {
    }
    
    Table84Table::Table84Table(CremaReader::itable& table)
    {
        this->Child_skullcapDeletable = new Table84Child_skullcapDeletableTable(table.dataset().tables()["Table84.Child_skullcapDeletable"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_skullcapDeletable->Rows, Table84SetChild_skullcapDeletable);
    }
    
    Table84Table::~Table84Table()
    {
        delete this->Child_skullcapDeletable;
    }
    
    void* Table84Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table84Row(row, ((Table84Table*)(table)));
    }
    
    const Table84Row* Table84Table::Find(char Eduino) const
    {
        return this->FindRow(Eduino);
    }
    
    Table_flangeChild1Row::Table_flangeChild1Row(CremaReader::irow& row, Table_flangeChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hauberk = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->tortoiseshell = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->costarring = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->cod = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->SEATO = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->shucker = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->gasworks = row.to_datetime(6);
        }
        this->SetKey(this->hauberk);
    }
    
    Table_flangeChild1Table::Table_flangeChild1Table()
    {
    }
    
    Table_flangeChild1Table::Table_flangeChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_flangeChild1Table::Table_flangeChild1Table(std::vector<Table_flangeChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_flangeChild1Table::~Table_flangeChild1Table()
    {
    }
    
    void* Table_flangeChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_flangeChild1Row(row, ((Table_flangeChild1Table*)(table)));
    }
    
    const Table_flangeChild1Row* Table_flangeChild1Table::Find(unsigned char hauberk) const
    {
        return this->FindRow(hauberk);
    }
    
    Table_flangeChild1Table Table_flangeRow::Child1Empty;
    
    Table_flangeRow::Table_flangeRow(CremaReader::irow& row, Table_flangeTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->meatpacking = row.to_double(0);
        if (row.has_value(1))
        {
            this->guardedness = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->Dugald = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->polymaths = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->thighbone = ((Type_Madison)(row.to_int32(4)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_flangeRow::Child1Empty);
        }
        this->SetKey(this->meatpacking);
    }
    
    void Table_flangeSetChild1(Table_flangeRow* target, const std::vector<Table_flangeChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_flangeChild1Table(childs);
    }
    
    Table_flangeTable::Table_flangeTable()
    {
    }
    
    Table_flangeTable::Table_flangeTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_flangeChild1Table(table.dataset().tables()["Table_flange.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_flangeSetChild1);
    }
    
    Table_flangeTable::~Table_flangeTable()
    {
        delete this->Child1;
    }
    
    void* Table_flangeTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_flangeRow(row, ((Table_flangeTable*)(table)));
    }
    
    const Table_flangeRow* Table_flangeTable::Find(double meatpacking) const
    {
        return this->FindRow(meatpacking);
    }
    
    Table_FonzChild1Row::Table_FonzChild1Row(CremaReader::irow& row, Table_FonzChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->Larsen = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->enforceable = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->aid = row.to_double(5);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table_FonzChild1Table::Table_FonzChild1Table()
    {
    }
    
    Table_FonzChild1Table::Table_FonzChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_FonzChild1Table::Table_FonzChild1Table(std::vector<Table_FonzChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_FonzChild1Table::~Table_FonzChild1Table()
    {
    }
    
    void* Table_FonzChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_FonzChild1Row(row, ((Table_FonzChild1Table*)(table)));
    }
    
    const Table_FonzChild1Row* Table_FonzChild1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table_FonzChild2Row::Table_FonzChild2Row(CremaReader::irow& row, Table_FonzChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->eruption = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->busty = ((Type_Attn)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Glory = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->Dayna = ((Type16)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->beckon = ((Type11)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Russian = row.to_int32(6);
        }
        if (row.has_value(7))
        {
            this->electrocution = row.to_boolean(7);
        }
        if (row.has_value(8))
        {
            this->allegorist = row.to_datetime(8);
        }
        if (row.has_value(9))
        {
            this->mizzen = ((Type71)(row.to_int32(9)));
        }
        this->SetKey(this->outgrip);
    }
    
    Table_FonzChild2Table::Table_FonzChild2Table()
    {
    }
    
    Table_FonzChild2Table::Table_FonzChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_FonzChild2Table::Table_FonzChild2Table(std::vector<Table_FonzChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_FonzChild2Table::~Table_FonzChild2Table()
    {
    }
    
    void* Table_FonzChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_FonzChild2Row(row, ((Table_FonzChild2Table*)(table)));
    }
    
    const Table_FonzChild2Row* Table_FonzChild2Table::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table_FonzChild3Row::Table_FonzChild3Row(CremaReader::irow& row, Table_FonzChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->dampen = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->hearing = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->bedsheets = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->Terri = row.to_int32(3);
        }
        this->Zagreb = row.to_boolean(4);
        if (row.has_value(5))
        {
            this->reading = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->leopardess = row.to_double(6);
        }
        this->SetKey(this->dampen, this->Zagreb);
    }
    
    Table_FonzChild3Table::Table_FonzChild3Table()
    {
    }
    
    Table_FonzChild3Table::Table_FonzChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_FonzChild3Table::Table_FonzChild3Table(std::vector<Table_FonzChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_FonzChild3Table::~Table_FonzChild3Table()
    {
    }
    
    void* Table_FonzChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_FonzChild3Row(row, ((Table_FonzChild3Table*)(table)));
    }
    
    const Table_FonzChild3Row* Table_FonzChild3Table::Find(unsigned long long dampen, bool Zagreb) const
    {
        return this->FindRow(dampen, Zagreb);
    }
    
    Table_FonzChild4Row::Table_FonzChild4Row(CremaReader::irow& row, Table_FonzChild4Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Slocum = row.to_single(0);
        this->SetKey(this->Slocum);
    }
    
    Table_FonzChild4Table::Table_FonzChild4Table()
    {
    }
    
    Table_FonzChild4Table::Table_FonzChild4Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_FonzChild4Table::Table_FonzChild4Table(std::vector<Table_FonzChild4Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_FonzChild4Table::~Table_FonzChild4Table()
    {
    }
    
    void* Table_FonzChild4Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_FonzChild4Row(row, ((Table_FonzChild4Table*)(table)));
    }
    
    const Table_FonzChild4Row* Table_FonzChild4Table::Find(float Slocum) const
    {
        return this->FindRow(Slocum);
    }
    
    Table_FonzChild1Table Table_FonzRow::Child1Empty;
    
    Table_FonzChild2Table Table_FonzRow::Child2Empty;
    
    Table_FonzChild3Table Table_FonzRow::Child3Empty;
    
    Table_FonzChild4Table Table_FonzRow::Child4Empty;
    
    Table_FonzRow::Table_FonzRow(CremaReader::irow& row, Table_FonzTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->operetta = row.to_int64(3);
        }
        if (row.has_value(4))
        {
            this->dammit = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->mystic = ((Type_supportedDeletable)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->tyrannizer = row.to_boolean(6);
        }
        if (row.has_value(7))
        {
            this->camp = row.to_uint64(7);
        }
        if (row.has_value(8))
        {
            this->Clint = row.to_uint64(8);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_FonzRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_FonzRow::Child2Empty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_FonzRow::Child3Empty);
        }
        if ((this->Child4 == nullptr))
        {
            this->Child4 = &(Table_FonzRow::Child4Empty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table_FonzSetChild1(Table_FonzRow* target, const std::vector<Table_FonzChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_FonzChild1Table(childs);
    }
    
    void Table_FonzSetChild2(Table_FonzRow* target, const std::vector<Table_FonzChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_FonzChild2Table(childs);
    }
    
    void Table_FonzSetChild3(Table_FonzRow* target, const std::vector<Table_FonzChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_FonzChild3Table(childs);
    }
    
    void Table_FonzSetChild4(Table_FonzRow* target, const std::vector<Table_FonzChild4Row*>& childs)
    {
        SetParent(target, childs);
        target->Child4 = new Table_FonzChild4Table(childs);
    }
    
    Table_FonzTable::Table_FonzTable()
    {
    }
    
    Table_FonzTable::Table_FonzTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_FonzChild1Table(table.dataset().tables()["Table_Fonz.Child1"]);
        this->Child2 = new Table_FonzChild2Table(table.dataset().tables()["Table_Fonz.Child2"]);
        this->Child3 = new Table_FonzChild3Table(table.dataset().tables()["Table_Fonz.Child3"]);
        this->Child4 = new Table_FonzChild4Table(table.dataset().tables()["Table_Fonz.Child4"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_FonzSetChild1);
        this->SetRelations(this->Child2->Rows, Table_FonzSetChild2);
        this->SetRelations(this->Child3->Rows, Table_FonzSetChild3);
        this->SetRelations(this->Child4->Rows, Table_FonzSetChild4);
    }
    
    Table_FonzTable::~Table_FonzTable()
    {
        delete this->Child1;
        delete this->Child2;
        delete this->Child3;
        delete this->Child4;
    }
    
    void* Table_FonzTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_FonzRow(row, ((Table_FonzTable*)(table)));
    }
    
    const Table_FonzRow* Table_FonzTable::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table_MelittaChild_MoseRow::Table_MelittaChild_MoseRow(CremaReader::irow& row, Table_MelittaChild_MoseTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->FTC = row.to_int16(0);
        if (row.has_value(1))
        {
            this->scrubbed = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->Cotton = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->bipedal = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->GPO = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->Shurlock = row.to_string(5);
        }
        this->Pammy = row.to_int16(6);
        if (row.has_value(7))
        {
            this->overstressed = row.to_datetime(7);
        }
        this->SetKey(this->FTC, this->Pammy);
    }
    
    Table_MelittaChild_MoseTable::Table_MelittaChild_MoseTable()
    {
    }
    
    Table_MelittaChild_MoseTable::Table_MelittaChild_MoseTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_MelittaChild_MoseTable::Table_MelittaChild_MoseTable(std::vector<Table_MelittaChild_MoseRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_MelittaChild_MoseTable::~Table_MelittaChild_MoseTable()
    {
    }
    
    void* Table_MelittaChild_MoseTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_MelittaChild_MoseRow(row, ((Table_MelittaChild_MoseTable*)(table)));
    }
    
    const Table_MelittaChild_MoseRow* Table_MelittaChild_MoseTable::Find(short FTC, short Pammy) const
    {
        return this->FindRow(FTC, Pammy);
    }
    
    Table_MelittaChild_MoseTable Table_MelittaRow::Child_MoseEmpty;
    
    Table_MelittaRow::Table_MelittaRow(CremaReader::irow& row, Table_MelittaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->snapback = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->caseworker = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Adonis = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->borderer = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->rattling = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->South = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->wagging = row.to_uint32(6);
        }
        if (row.has_value(7))
        {
            this->deleterious = row.to_duration(7);
        }
        if (row.has_value(8))
        {
            this->Dalmatian = row.to_int64(8);
        }
        if ((this->Child_Mose == nullptr))
        {
            this->Child_Mose = &(Table_MelittaRow::Child_MoseEmpty);
        }
        this->SetKey(this->snapback);
    }
    
    void Table_MelittaSetChild_Mose(Table_MelittaRow* target, const std::vector<Table_MelittaChild_MoseRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Mose = new Table_MelittaChild_MoseTable(childs);
    }
    
    Table_MelittaTable::Table_MelittaTable()
    {
    }
    
    Table_MelittaTable::Table_MelittaTable(CremaReader::itable& table)
    {
        this->Child_Mose = new Table_MelittaChild_MoseTable(table.dataset().tables()["Table_Melitta.Child_Mose"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Mose->Rows, Table_MelittaSetChild_Mose);
    }
    
    Table_MelittaTable::~Table_MelittaTable()
    {
        delete this->Child_Mose;
    }
    
    void* Table_MelittaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_MelittaRow(row, ((Table_MelittaTable*)(table)));
    }
    
    const Table_MelittaRow* Table_MelittaTable::Find(unsigned int snapback) const
    {
        return this->FindRow(snapback);
    }
    
    Table_quartileRow::Table_quartileRow(CremaReader::irow& row, Table_quartileTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->travelogue = row.to_single(0);
        if (row.has_value(1))
        {
            this->divider = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->schmuck = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->epistemic = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->Gaulish = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->apportionment = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->toxin = ((Type_Arlan)(row.to_int32(6)));
        }
        this->SetKey(this->travelogue);
    }
    
    Table_quartileTable::Table_quartileTable()
    {
    }
    
    Table_quartileTable::Table_quartileTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_quartileTable::~Table_quartileTable()
    {
    }
    
    void* Table_quartileTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_quartileRow(row, ((Table_quartileTable*)(table)));
    }
    
    const Table_quartileRow* Table_quartileTable::Find(float travelogue) const
    {
        return this->FindRow(travelogue);
    }
    
    Table12Row::Table12Row(CremaReader::irow& row, Table12Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->corpsman = row.to_double(0);
        if (row.has_value(1))
        {
            this->Araucanian = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Kaposi = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->hyperemia = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->pensiveness = row.to_single(4);
        }
        this->jetting = row.to_boolean(5);
        if (row.has_value(6))
        {
            this->babe = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->clears = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->codetermine = row.to_duration(8);
        }
        this->SetKey(this->corpsman, this->jetting);
    }
    
    Table12Table::Table12Table()
    {
    }
    
    Table12Table::Table12Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table12Table::~Table12Table()
    {
    }
    
    void* Table12Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table12Row(row, ((Table12Table*)(table)));
    }
    
    const Table12Row* Table12Table::Find(double corpsman, bool jetting) const
    {
        return this->FindRow(corpsman, jetting);
    }
    
    Table202Row::Table202Row(CremaReader::irow& row, Table202Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->kyle = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->strumpet = ((Type_primitiveness)(row.to_int32(1)));
        }
        this->SetKey(this->kyle);
    }
    
    Table202Table::Table202Table()
    {
    }
    
    Table202Table::Table202Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table202Table::~Table202Table()
    {
    }
    
    void* Table202Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table202Row(row, ((Table202Table*)(table)));
    }
    
    const Table202Row* Table202Table::Find(bool kyle) const
    {
        return this->FindRow(kyle);
    }
    
    Table3Row::Table3Row(CremaReader::irow& row, Table3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->spiker = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Celie = ((Type_teapot)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->genetic = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->perpetuate = row.to_uint8(3);
        }
        this->SetKey(this->spiker);
    }
    
    Table3Table::Table3Table()
    {
    }
    
    Table3Table::Table3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table3Table::~Table3Table()
    {
    }
    
    void* Table3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table3Row(row, ((Table3Table*)(table)));
    }
    
    const Table3Row* Table3Table::Find(unsigned short spiker) const
    {
        return this->FindRow(spiker);
    }
    
    Table63Row::Table63Row(CremaReader::irow& row, Table63Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->signaler = row.to_string(0);
        this->truster = row.to_single(1);
        if (row.has_value(2))
        {
            this->retro = row.to_int64(2);
        }
        this->SetKey(this->signaler, this->truster);
    }
    
    Table63Table::Table63Table()
    {
    }
    
    Table63Table::Table63Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table63Table::~Table63Table()
    {
    }
    
    void* Table63Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table63Row(row, ((Table63Table*)(table)));
    }
    
    const Table63Row* Table63Table::Find(const std::string& signaler, float truster) const
    {
        return this->FindRow(signaler, truster);
    }
    
    Table136Row::Table136Row(CremaReader::irow& row, Table136Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Whitaker = ((Type_Attn)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->fever = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->Waverley = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->Merline = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->Phineas = row.to_int16(4);
        }
        if (row.has_value(5))
        {
            this->Rochella = row.to_int32(5);
        }
        this->SetKey(this->Whitaker);
    }
    
    Table136Table::Table136Table()
    {
    }
    
    Table136Table::Table136Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table136Table::~Table136Table()
    {
    }
    
    void* Table136Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table136Row(row, ((Table136Table*)(table)));
    }
    
    const Table136Row* Table136Table::Find(Type_Attn Whitaker) const
    {
        return this->FindRow(Whitaker);
    }
    
    Table66Row::Table66Row(CremaReader::irow& row, Table66Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Rhineland = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Dukie = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->optimizer = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->Chilean = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->mismanagement = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->Biddle = row.to_int16(5);
        }
        this->SetKey(this->Rhineland);
    }
    
    Table66Table::Table66Table()
    {
    }
    
    Table66Table::Table66Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table66Table::~Table66Table()
    {
    }
    
    void* Table66Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table66Row(row, ((Table66Table*)(table)));
    }
    
    const Table66Row* Table66Table::Find(char Rhineland) const
    {
        return this->FindRow(Rhineland);
    }
    
    Table113Row::Table113Row(CremaReader::irow& row, Table113Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Gondwanaland = row.to_int64(0);
        this->SetKey(this->Gondwanaland);
    }
    
    Table113Table::Table113Table()
    {
    }
    
    Table113Table::Table113Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table113Table::~Table113Table()
    {
    }
    
    void* Table113Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table113Row(row, ((Table113Table*)(table)));
    }
    
    const Table113Row* Table113Table::Find(long long Gondwanaland) const
    {
        return this->FindRow(Gondwanaland);
    }
    
    Table52Row::Table52Row(CremaReader::irow& row, Table52Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->humanest = row.to_int8(0);
        this->SetKey(this->humanest);
    }
    
    Table52Table::Table52Table()
    {
    }
    
    Table52Table::Table52Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table52Table::~Table52Table()
    {
    }
    
    void* Table52Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table52Row(row, ((Table52Table*)(table)));
    }
    
    const Table52Row* Table52Table::Find(char humanest) const
    {
        return this->FindRow(humanest);
    }
    
    Table_glideRow::Table_glideRow(CremaReader::irow& row, Table_glideTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->misstep = ((Type8)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Alistair = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->birth = ((Type15)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Abdel = row.to_int16(3);
        }
        if (row.has_value(4))
        {
            this->cousinly = row.to_uint64(4);
        }
        if (row.has_value(5))
        {
            this->Paley = row.to_int16(5);
        }
        this->SetKey(this->misstep);
    }
    
    Table_glideTable::Table_glideTable()
    {
    }
    
    Table_glideTable::Table_glideTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_glideTable::~Table_glideTable()
    {
    }
    
    void* Table_glideTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_glideRow(row, ((Table_glideTable*)(table)));
    }
    
    const Table_glideRow* Table_glideTable::Find(Type8 misstep) const
    {
        return this->FindRow(misstep);
    }
    
    Table179Row::Table179Row(CremaReader::irow& row, Table179Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->treacherousness = row.to_string(0);
        if (row.has_value(1))
        {
            this->McGrath = ((Type_Jenelle)(row.to_int32(1)));
        }
        this->Pan = row.to_single(2);
        if (row.has_value(3))
        {
            this->bushmaster = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->stamen = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->thoughtlessness = row.to_int32(5);
        }
        this->SetKey(this->treacherousness, this->Pan);
    }
    
    Table179Table::Table179Table()
    {
    }
    
    Table179Table::Table179Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table179Table::~Table179Table()
    {
    }
    
    void* Table179Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table179Row(row, ((Table179Table*)(table)));
    }
    
    const Table179Row* Table179Table::Find(const std::string& treacherousness, float Pan) const
    {
        return this->FindRow(treacherousness, Pan);
    }
    
    Table_winglessChild2Row::Table_winglessChild2Row(CremaReader::irow& row, Table_winglessChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Arawakan = row.to_single(0);
        if (row.has_value(1))
        {
            this->candelabra = row.to_int64(1);
        }
        if (row.has_value(2))
        {
            this->polonium = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->perigee = row.to_int16(3);
        }
        this->SetKey(this->Arawakan);
    }
    
    Table_winglessChild2Table::Table_winglessChild2Table()
    {
    }
    
    Table_winglessChild2Table::Table_winglessChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_winglessChild2Table::Table_winglessChild2Table(std::vector<Table_winglessChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_winglessChild2Table::~Table_winglessChild2Table()
    {
    }
    
    void* Table_winglessChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_winglessChild2Row(row, ((Table_winglessChild2Table*)(table)));
    }
    
    const Table_winglessChild2Row* Table_winglessChild2Table::Find(float Arawakan) const
    {
        return this->FindRow(Arawakan);
    }
    
    Table_winglessChild2Table Table_winglessRow::Child2Empty;
    
    Table_winglessRow::Table_winglessRow(CremaReader::irow& row, Table_winglessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->flaxseed = row.to_uint8(0);
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_winglessRow::Child2Empty);
        }
        this->SetKey(this->flaxseed);
    }
    
    void Table_winglessSetChild2(Table_winglessRow* target, const std::vector<Table_winglessChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_winglessChild2Table(childs);
    }
    
    Table_winglessTable::Table_winglessTable()
    {
    }
    
    Table_winglessTable::Table_winglessTable(CremaReader::itable& table)
    {
        this->Child2 = new Table_winglessChild2Table(table.dataset().tables()["Table_wingless.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child2->Rows, Table_winglessSetChild2);
    }
    
    Table_winglessTable::~Table_winglessTable()
    {
        delete this->Child2;
    }
    
    void* Table_winglessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_winglessRow(row, ((Table_winglessTable*)(table)));
    }
    
    const Table_winglessRow* Table_winglessTable::Find(unsigned char flaxseed) const
    {
        return this->FindRow(flaxseed);
    }
    
    Table137Row::Table137Row(CremaReader::irow& row, Table137Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Rose = ((Type56)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Filippa = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->Donnamarie = row.to_int32(2);
        }
        this->SetKey(this->Rose);
    }
    
    Table137Table::Table137Table()
    {
    }
    
    Table137Table::Table137Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table137Table::~Table137Table()
    {
    }
    
    void* Table137Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table137Row(row, ((Table137Table*)(table)));
    }
    
    const Table137Row* Table137Table::Find(Type56 Rose) const
    {
        return this->FindRow(Rose);
    }
    
    Table_ItoChild1Row::Table_ItoChild1Row(CremaReader::irow& row, Table_ItoChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->schism = row.to_int64(0);
        if (row.has_value(1))
        {
            this->BBC = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->Ge = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->voluntariness = ((Type_canted)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->grubbed = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->amicableness = row.to_double(5);
        }
        if (row.has_value(6))
        {
            this->gratuity = row.to_duration(6);
        }
        this->SetKey(this->schism);
    }
    
    Table_ItoChild1Table::Table_ItoChild1Table()
    {
    }
    
    Table_ItoChild1Table::Table_ItoChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_ItoChild1Table::Table_ItoChild1Table(std::vector<Table_ItoChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_ItoChild1Table::~Table_ItoChild1Table()
    {
    }
    
    void* Table_ItoChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_ItoChild1Row(row, ((Table_ItoChild1Table*)(table)));
    }
    
    const Table_ItoChild1Row* Table_ItoChild1Table::Find(long long schism) const
    {
        return this->FindRow(schism);
    }
    
    Table_ItoChild2Row::Table_ItoChild2Row(CremaReader::irow& row, Table_ItoChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->gos = row.to_int8(0);
        if (row.has_value(1))
        {
            this->photolysis = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Liberian = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->Palmyra = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->airbus = row.to_datetime(4);
        }
        this->SetKey(this->gos);
    }
    
    Table_ItoChild2Table::Table_ItoChild2Table()
    {
    }
    
    Table_ItoChild2Table::Table_ItoChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_ItoChild2Table::Table_ItoChild2Table(std::vector<Table_ItoChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_ItoChild2Table::~Table_ItoChild2Table()
    {
    }
    
    void* Table_ItoChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_ItoChild2Row(row, ((Table_ItoChild2Table*)(table)));
    }
    
    const Table_ItoChild2Row* Table_ItoChild2Table::Find(char gos) const
    {
        return this->FindRow(gos);
    }
    
    Table_ItoChild1Table Table_ItoRow::Child1Empty;
    
    Table_ItoChild2Table Table_ItoRow::Child2Empty;
    
    Table_ItoRow::Table_ItoRow(CremaReader::irow& row, Table_ItoTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->reprehensible = ((Type3)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->headmastership = row.to_datetime(1);
        }
        this->Gienah = row.to_int16(2);
        if (row.has_value(3))
        {
            this->capo = row.to_single(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_ItoRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_ItoRow::Child2Empty);
        }
        this->SetKey(this->reprehensible, this->Gienah);
    }
    
    void Table_ItoSetChild1(Table_ItoRow* target, const std::vector<Table_ItoChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_ItoChild1Table(childs);
    }
    
    void Table_ItoSetChild2(Table_ItoRow* target, const std::vector<Table_ItoChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_ItoChild2Table(childs);
    }
    
    Table_ItoTable::Table_ItoTable()
    {
    }
    
    Table_ItoTable::Table_ItoTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_ItoChild1Table(table.dataset().tables()["Table_Ito.Child1"]);
        this->Child2 = new Table_ItoChild2Table(table.dataset().tables()["Table_Ito.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_ItoSetChild1);
        this->SetRelations(this->Child2->Rows, Table_ItoSetChild2);
    }
    
    Table_ItoTable::~Table_ItoTable()
    {
        delete this->Child1;
        delete this->Child2;
    }
    
    void* Table_ItoTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_ItoRow(row, ((Table_ItoTable*)(table)));
    }
    
    const Table_ItoRow* Table_ItoTable::Find(Type3 reprehensible, short Gienah) const
    {
        return this->FindRow(reprehensible, Gienah);
    }
    
    Table_KatherynRow::Table_KatherynRow(CremaReader::irow& row, Table_KatherynTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->seemliness = row.to_single(0);
        if (row.has_value(1))
        {
            this->sandpit = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->carbonization = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->whacker = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->happing = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->considerable = row.to_int64(5);
        }
        if (row.has_value(6))
        {
            this->mis = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->impairer = row.to_string(7);
        }
        this->SetKey(this->seemliness);
    }
    
    Table_KatherynTable::Table_KatherynTable()
    {
    }
    
    Table_KatherynTable::Table_KatherynTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_KatherynTable::~Table_KatherynTable()
    {
    }
    
    void* Table_KatherynTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_KatherynRow(row, ((Table_KatherynTable*)(table)));
    }
    
    const Table_KatherynRow* Table_KatherynTable::Find(float seemliness) const
    {
        return this->FindRow(seemliness);
    }
    
    Table_LuciusChild1Row::Table_LuciusChild1Row(CremaReader::irow& row, Table_LuciusChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->pill = row.to_boolean(3);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table_LuciusChild1Table::Table_LuciusChild1Table()
    {
    }
    
    Table_LuciusChild1Table::Table_LuciusChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LuciusChild1Table::Table_LuciusChild1Table(std::vector<Table_LuciusChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_LuciusChild1Table::~Table_LuciusChild1Table()
    {
    }
    
    void* Table_LuciusChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LuciusChild1Row(row, ((Table_LuciusChild1Table*)(table)));
    }
    
    const Table_LuciusChild1Row* Table_LuciusChild1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table_LuciusChild_newsprintRow::Table_LuciusChild_newsprintRow(CremaReader::irow& row, Table_LuciusChild_newsprintTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->condominium = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->guiltlessness = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->splash = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->entrapping = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->Hamnet = row.to_datetime(5);
        }
        this->SetKey(this->outgrip);
    }
    
    Table_LuciusChild_newsprintTable::Table_LuciusChild_newsprintTable()
    {
    }
    
    Table_LuciusChild_newsprintTable::Table_LuciusChild_newsprintTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LuciusChild_newsprintTable::Table_LuciusChild_newsprintTable(std::vector<Table_LuciusChild_newsprintRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_LuciusChild_newsprintTable::~Table_LuciusChild_newsprintTable()
    {
    }
    
    void* Table_LuciusChild_newsprintTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LuciusChild_newsprintRow(row, ((Table_LuciusChild_newsprintTable*)(table)));
    }
    
    const Table_LuciusChild_newsprintRow* Table_LuciusChild_newsprintTable::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table_LuciusChild_nevusRow::Table_LuciusChild_nevusRow(CremaReader::irow& row, Table_LuciusChild_nevusTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->repetition = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->phonetician = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->Nanni = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->monographs = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->humus = row.to_int64(4);
        }
        this->SetKey(this->repetition);
    }
    
    Table_LuciusChild_nevusTable::Table_LuciusChild_nevusTable()
    {
    }
    
    Table_LuciusChild_nevusTable::Table_LuciusChild_nevusTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LuciusChild_nevusTable::Table_LuciusChild_nevusTable(std::vector<Table_LuciusChild_nevusRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_LuciusChild_nevusTable::~Table_LuciusChild_nevusTable()
    {
    }
    
    void* Table_LuciusChild_nevusTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LuciusChild_nevusRow(row, ((Table_LuciusChild_nevusTable*)(table)));
    }
    
    const Table_LuciusChild_nevusRow* Table_LuciusChild_nevusTable::Find(unsigned char repetition) const
    {
        return this->FindRow(repetition);
    }
    
    Table_LuciusChild3Row::Table_LuciusChild3Row(CremaReader::irow& row, Table_LuciusChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->vaginae = row.to_int64(0);
        if (row.has_value(1))
        {
            this->obvious = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->sachet = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->underpayment = ((Type11)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->axle = row.to_int8(4);
        }
        this->SetKey(this->vaginae);
    }
    
    Table_LuciusChild3Table::Table_LuciusChild3Table()
    {
    }
    
    Table_LuciusChild3Table::Table_LuciusChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LuciusChild3Table::Table_LuciusChild3Table(std::vector<Table_LuciusChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_LuciusChild3Table::~Table_LuciusChild3Table()
    {
    }
    
    void* Table_LuciusChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LuciusChild3Row(row, ((Table_LuciusChild3Table*)(table)));
    }
    
    const Table_LuciusChild3Row* Table_LuciusChild3Table::Find(long long vaginae) const
    {
        return this->FindRow(vaginae);
    }
    
    Table_LuciusChild2Row::Table_LuciusChild2Row(CremaReader::irow& row, Table_LuciusChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Enos = row.to_single(0);
        if (row.has_value(1))
        {
            this->pervasive = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->lubricator = row.to_boolean(2);
        }
        this->hallmark = row.to_string(3);
        this->SetKey(this->Enos, this->hallmark);
    }
    
    Table_LuciusChild2Table::Table_LuciusChild2Table()
    {
    }
    
    Table_LuciusChild2Table::Table_LuciusChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LuciusChild2Table::Table_LuciusChild2Table(std::vector<Table_LuciusChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_LuciusChild2Table::~Table_LuciusChild2Table()
    {
    }
    
    void* Table_LuciusChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LuciusChild2Row(row, ((Table_LuciusChild2Table*)(table)));
    }
    
    const Table_LuciusChild2Row* Table_LuciusChild2Table::Find(float Enos, const std::string& hallmark) const
    {
        return this->FindRow(Enos, hallmark);
    }
    
    Table_LuciusChild1Table Table_LuciusRow::Child1Empty;
    
    Table_LuciusChild_newsprintTable Table_LuciusRow::Child_newsprintEmpty;
    
    Table_LuciusChild_nevusTable Table_LuciusRow::Child_nevusEmpty;
    
    Table_LuciusChild3Table Table_LuciusRow::Child3Empty;
    
    Table_LuciusChild2Table Table_LuciusRow::Child2Empty;
    
    Table_LuciusRow::Table_LuciusRow(CremaReader::irow& row, Table_LuciusTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->operetta = row.to_int64(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_LuciusRow::Child1Empty);
        }
        if ((this->Child_newsprint == nullptr))
        {
            this->Child_newsprint = &(Table_LuciusRow::Child_newsprintEmpty);
        }
        if ((this->Child_nevus == nullptr))
        {
            this->Child_nevus = &(Table_LuciusRow::Child_nevusEmpty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_LuciusRow::Child3Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_LuciusRow::Child2Empty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table_LuciusSetChild1(Table_LuciusRow* target, const std::vector<Table_LuciusChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_LuciusChild1Table(childs);
    }
    
    void Table_LuciusSetChild_newsprint(Table_LuciusRow* target, const std::vector<Table_LuciusChild_newsprintRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_newsprint = new Table_LuciusChild_newsprintTable(childs);
    }
    
    void Table_LuciusSetChild_nevus(Table_LuciusRow* target, const std::vector<Table_LuciusChild_nevusRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_nevus = new Table_LuciusChild_nevusTable(childs);
    }
    
    void Table_LuciusSetChild3(Table_LuciusRow* target, const std::vector<Table_LuciusChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_LuciusChild3Table(childs);
    }
    
    void Table_LuciusSetChild2(Table_LuciusRow* target, const std::vector<Table_LuciusChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_LuciusChild2Table(childs);
    }
    
    Table_LuciusTable::Table_LuciusTable()
    {
    }
    
    Table_LuciusTable::Table_LuciusTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_LuciusChild1Table(table.dataset().tables()["Table_Lucius.Child1"]);
        this->Child_newsprint = new Table_LuciusChild_newsprintTable(table.dataset().tables()["Table_Lucius.Child_newsprint"]);
        this->Child_nevus = new Table_LuciusChild_nevusTable(table.dataset().tables()["Table_Lucius.Child_nevus"]);
        this->Child3 = new Table_LuciusChild3Table(table.dataset().tables()["Table_Lucius.Child3"]);
        this->Child2 = new Table_LuciusChild2Table(table.dataset().tables()["Table_Lucius.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_LuciusSetChild1);
        this->SetRelations(this->Child_newsprint->Rows, Table_LuciusSetChild_newsprint);
        this->SetRelations(this->Child_nevus->Rows, Table_LuciusSetChild_nevus);
        this->SetRelations(this->Child3->Rows, Table_LuciusSetChild3);
        this->SetRelations(this->Child2->Rows, Table_LuciusSetChild2);
    }
    
    Table_LuciusTable::~Table_LuciusTable()
    {
        delete this->Child1;
        delete this->Child_newsprint;
        delete this->Child_nevus;
        delete this->Child3;
        delete this->Child2;
    }
    
    void* Table_LuciusTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LuciusRow(row, ((Table_LuciusTable*)(table)));
    }
    
    const Table_LuciusRow* Table_LuciusTable::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table_MaxwellRow::Table_MaxwellRow(CremaReader::irow& row, Table_MaxwellTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Argentina = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->wingding = ((Type_Multan)(row.to_int32(1)));
        }
        this->SetKey(this->Argentina);
    }
    
    Table_MaxwellTable::Table_MaxwellTable()
    {
    }
    
    Table_MaxwellTable::Table_MaxwellTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_MaxwellTable::~Table_MaxwellTable()
    {
    }
    
    void* Table_MaxwellTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_MaxwellRow(row, ((Table_MaxwellTable*)(table)));
    }
    
    const Table_MaxwellRow* Table_MaxwellTable::Find(unsigned int Argentina) const
    {
        return this->FindRow(Argentina);
    }
    
    Table_bangRow::Table_bangRow(CremaReader::irow& row, Table_bangTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->pylori = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->Kylen = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->zigzag = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->Appolonia = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->sahib = row.to_int64(4);
        }
        this->SetKey(this->pylori);
    }
    
    Table_bangTable::Table_bangTable()
    {
    }
    
    Table_bangTable::Table_bangTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_bangTable::~Table_bangTable()
    {
    }
    
    void* Table_bangTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_bangRow(row, ((Table_bangTable*)(table)));
    }
    
    const Table_bangRow* Table_bangTable::Find(unsigned char pylori) const
    {
        return this->FindRow(pylori);
    }
    
    Table_linemanChild_MoseRow::Table_linemanChild_MoseRow(CremaReader::irow& row, Table_linemanChild_MoseTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->FTC = row.to_int16(0);
        if (row.has_value(1))
        {
            this->scrubbed = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->Cotton = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->bipedal = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->GPO = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->Shurlock = row.to_string(5);
        }
        this->Pammy = row.to_int16(6);
        if (row.has_value(7))
        {
            this->overstressed = row.to_datetime(7);
        }
        this->SetKey(this->FTC, this->Pammy);
    }
    
    Table_linemanChild_MoseTable::Table_linemanChild_MoseTable()
    {
    }
    
    Table_linemanChild_MoseTable::Table_linemanChild_MoseTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_linemanChild_MoseTable::Table_linemanChild_MoseTable(std::vector<Table_linemanChild_MoseRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_linemanChild_MoseTable::~Table_linemanChild_MoseTable()
    {
    }
    
    void* Table_linemanChild_MoseTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_linemanChild_MoseRow(row, ((Table_linemanChild_MoseTable*)(table)));
    }
    
    const Table_linemanChild_MoseRow* Table_linemanChild_MoseTable::Find(short FTC, short Pammy) const
    {
        return this->FindRow(FTC, Pammy);
    }
    
    Table_linemanChild_MoseTable Table_linemanRow::Child_MoseEmpty;
    
    Table_linemanRow::Table_linemanRow(CremaReader::irow& row, Table_linemanTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->snapback = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->caseworker = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Adonis = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->borderer = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->rattling = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->South = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->wagging = row.to_uint32(6);
        }
        if (row.has_value(7))
        {
            this->deleterious = row.to_duration(7);
        }
        if (row.has_value(8))
        {
            this->Dalmatian = row.to_int64(8);
        }
        if ((this->Child_Mose == nullptr))
        {
            this->Child_Mose = &(Table_linemanRow::Child_MoseEmpty);
        }
        this->SetKey(this->snapback);
    }
    
    void Table_linemanSetChild_Mose(Table_linemanRow* target, const std::vector<Table_linemanChild_MoseRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Mose = new Table_linemanChild_MoseTable(childs);
    }
    
    Table_linemanTable::Table_linemanTable()
    {
    }
    
    Table_linemanTable::Table_linemanTable(CremaReader::itable& table)
    {
        this->Child_Mose = new Table_linemanChild_MoseTable(table.dataset().tables()["Table_lineman.Child_Mose"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Mose->Rows, Table_linemanSetChild_Mose);
    }
    
    Table_linemanTable::~Table_linemanTable()
    {
        delete this->Child_Mose;
    }
    
    void* Table_linemanTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_linemanRow(row, ((Table_linemanTable*)(table)));
    }
    
    const Table_linemanRow* Table_linemanTable::Find(unsigned int snapback) const
    {
        return this->FindRow(snapback);
    }
    
    Table100Row::Table100Row(CremaReader::irow& row, Table100Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->nosh = row.to_duration(0);
        if (row.has_value(1))
        {
            this->Dee = ((Type_Arlan)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->majesty = row.to_boolean(2);
        }
        this->SetKey(this->nosh);
    }
    
    Table100Table::Table100Table()
    {
    }
    
    Table100Table::Table100Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table100Table::~Table100Table()
    {
    }
    
    void* Table100Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table100Row(row, ((Table100Table*)(table)));
    }
    
    const Table100Row* Table100Table::Find(int nosh) const
    {
        return this->FindRow(nosh);
    }
    
    Table74Row::Table74Row(CremaReader::irow& row, Table74Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->referral = row.to_double(0);
        if (row.has_value(1))
        {
            this->Tana = ((Type3)(row.to_int32(1)));
        }
        this->baryon = ((Type4)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->Shawnee = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->ostensibly = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->Chrissie = row.to_string(5);
        }
        this->needlewoman = row.to_string(6);
        this->SetKey(this->referral, this->baryon, this->needlewoman);
    }
    
    Table74Table::Table74Table()
    {
    }
    
    Table74Table::Table74Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table74Table::~Table74Table()
    {
    }
    
    void* Table74Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table74Row(row, ((Table74Table*)(table)));
    }
    
    const Table74Row* Table74Table::Find(double referral, Type4 baryon, const std::string& needlewoman) const
    {
        return this->FindRow(referral, baryon, needlewoman);
    }
    
    Table97Row::Table97Row(CremaReader::irow& row, Table97Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->undereducated = ((Type25)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->wrap = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->LOGO = row.to_uint16(2);
        }
        if (row.has_value(3))
        {
            this->frozen = row.to_datetime(3);
        }
        this->rummage = row.to_uint16(4);
        if (row.has_value(5))
        {
            this->disparate = row.to_uint64(5);
        }
        if (row.has_value(6))
        {
            this->slurrying = row.to_string(6);
        }
        if (row.has_value(7))
        {
            this->aeronautic = row.to_double(7);
        }
        this->SetKey(this->undereducated, this->rummage);
    }
    
    Table97Table::Table97Table()
    {
    }
    
    Table97Table::Table97Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table97Table::~Table97Table()
    {
    }
    
    void* Table97Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table97Row(row, ((Table97Table*)(table)));
    }
    
    const Table97Row* Table97Table::Find(Type25 undereducated, unsigned short rummage) const
    {
        return this->FindRow(undereducated, rummage);
    }
    
    Table_archaicallyRow::Table_archaicallyRow(CremaReader::irow& row, Table_archaicallyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Bostitch = ((Type40)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Bonnee = row.to_duration(1);
        }
        this->Mose = ((Type_HeraclitusDeletable)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->valuable = row.to_double(3);
        }
        this->Diahann = row.to_datetime(4);
        if (row.has_value(5))
        {
            this->pizzeria = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->bipartisanship = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->seagoing = row.to_string(7);
        }
        this->SetKey(this->Bostitch, this->Mose, this->Diahann);
    }
    
    Table_archaicallyTable::Table_archaicallyTable()
    {
    }
    
    Table_archaicallyTable::Table_archaicallyTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_archaicallyTable::~Table_archaicallyTable()
    {
    }
    
    void* Table_archaicallyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_archaicallyRow(row, ((Table_archaicallyTable*)(table)));
    }
    
    const Table_archaicallyRow* Table_archaicallyTable::Find(Type40 Bostitch, Type_HeraclitusDeletable Mose, time_t Diahann) const
    {
        return this->FindRow(Bostitch, Mose, Diahann);
    }
    
    Table_codenameRow::Table_codenameRow(CremaReader::irow& row, Table_codenameTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->cuttlefish = row.to_duration(0);
        if (row.has_value(1))
        {
            this->cruse = row.to_uint64(1);
        }
        this->yonder = row.to_uint64(2);
        if (row.has_value(3))
        {
            this->Michell = ((Type25)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->rabbet = ((Type_primitiveness)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->Sabra = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->vulcanization = row.to_datetime(6);
        }
        if (row.has_value(7))
        {
            this->Ilka = row.to_uint16(7);
        }
        if (row.has_value(8))
        {
            this->Waldheim = row.to_boolean(8);
        }
        if (row.has_value(9))
        {
            this->Curtice = row.to_duration(9);
        }
        if (row.has_value(10))
        {
            this->anticlerical = row.to_int16(10);
        }
        if (row.has_value(11))
        {
            this->Pam = row.to_single(11);
        }
        this->SetKey(this->cuttlefish, this->yonder);
    }
    
    Table_codenameTable::Table_codenameTable()
    {
    }
    
    Table_codenameTable::Table_codenameTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_codenameTable::~Table_codenameTable()
    {
    }
    
    void* Table_codenameTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_codenameRow(row, ((Table_codenameTable*)(table)));
    }
    
    const Table_codenameRow* Table_codenameTable::Find(int cuttlefish, unsigned long long yonder) const
    {
        return this->FindRow(cuttlefish, yonder);
    }
    
    Table_protectivenessRow::Table_protectivenessRow(CremaReader::irow& row, Table_protectivenessTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lumberyard = ((Type6)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Carl = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->firearm = row.to_boolean(2);
        }
        this->uncap = row.to_int8(3);
        if (row.has_value(4))
        {
            this->chairwoman = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->Bassett = ((Type8)(row.to_int32(5)));
        }
        this->SetKey(this->lumberyard, this->uncap);
    }
    
    Table_protectivenessTable::Table_protectivenessTable()
    {
    }
    
    Table_protectivenessTable::Table_protectivenessTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_protectivenessTable::~Table_protectivenessTable()
    {
    }
    
    void* Table_protectivenessTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_protectivenessRow(row, ((Table_protectivenessTable*)(table)));
    }
    
    const Table_protectivenessRow* Table_protectivenessTable::Find(Type6 lumberyard, char uncap) const
    {
        return this->FindRow(lumberyard, uncap);
    }
    
    Table_dissuasiveRow::Table_dissuasiveRow(CremaReader::irow& row, Table_dissuasiveTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->signaler = row.to_string(0);
        this->truster = row.to_single(1);
        if (row.has_value(2))
        {
            this->retro = row.to_int64(2);
        }
        this->SetKey(this->signaler, this->truster);
    }
    
    Table_dissuasiveTable::Table_dissuasiveTable()
    {
    }
    
    Table_dissuasiveTable::Table_dissuasiveTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_dissuasiveTable::~Table_dissuasiveTable()
    {
    }
    
    void* Table_dissuasiveTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_dissuasiveRow(row, ((Table_dissuasiveTable*)(table)));
    }
    
    const Table_dissuasiveRow* Table_dissuasiveTable::Find(const std::string& signaler, float truster) const
    {
        return this->FindRow(signaler, truster);
    }
    
    Table110Row::Table110Row(CremaReader::irow& row, Table110Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Mariel = row.to_double(0);
        if (row.has_value(1))
        {
            this->reversioner = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->epigraph = row.to_int16(2);
        }
        if (row.has_value(3))
        {
            this->satire = ((Type27)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->Oglethorpe = ((Type50)(row.to_int32(4)));
        }
        this->mappable = row.to_single(5);
        if (row.has_value(6))
        {
            this->misquote = ((Type3)(row.to_int32(6)));
        }
        this->SetKey(this->Mariel, this->mappable);
    }
    
    Table110Table::Table110Table()
    {
    }
    
    Table110Table::Table110Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table110Table::~Table110Table()
    {
    }
    
    void* Table110Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table110Row(row, ((Table110Table*)(table)));
    }
    
    const Table110Row* Table110Table::Find(double Mariel, float mappable) const
    {
        return this->FindRow(Mariel, mappable);
    }
    
    Table_relentChild_leafletRow::Table_relentChild_leafletRow(CremaReader::irow& row, Table_relentChild_leafletTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->uninteresting = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->Lafitte = row.to_double(1);
        }
        this->feater = row.to_double(2);
        if (row.has_value(3))
        {
            this->doggone = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->midi = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->filament = ((Type27)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Zorana = ((Type_consortia)(row.to_int32(6)));
        }
        this->highish = ((Type_Meiji)(row.to_int32(7)));
        this->SetKey(this->uninteresting, this->feater, this->highish);
    }
    
    Table_relentChild_leafletTable::Table_relentChild_leafletTable()
    {
    }
    
    Table_relentChild_leafletTable::Table_relentChild_leafletTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_relentChild_leafletTable::Table_relentChild_leafletTable(std::vector<Table_relentChild_leafletRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_relentChild_leafletTable::~Table_relentChild_leafletTable()
    {
    }
    
    void* Table_relentChild_leafletTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_relentChild_leafletRow(row, ((Table_relentChild_leafletTable*)(table)));
    }
    
    const Table_relentChild_leafletRow* Table_relentChild_leafletTable::Find(unsigned char uninteresting, double feater, Type_Meiji highish) const
    {
        return this->FindRow(uninteresting, feater, highish);
    }
    
    Table_relentChild_leafletTable Table_relentRow::Child_leafletEmpty;
    
    Table_relentRow::Table_relentRow(CremaReader::irow& row, Table_relentTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->adviser = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Peterson = ((Type_pledge)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->cumuli = ((Type24)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->analyzed = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->scowler = row.to_int64(4);
        }
        if ((this->Child_leaflet == nullptr))
        {
            this->Child_leaflet = &(Table_relentRow::Child_leafletEmpty);
        }
        this->SetKey(this->adviser);
    }
    
    void Table_relentSetChild_leaflet(Table_relentRow* target, const std::vector<Table_relentChild_leafletRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_leaflet = new Table_relentChild_leafletTable(childs);
    }
    
    Table_relentTable::Table_relentTable()
    {
    }
    
    Table_relentTable::Table_relentTable(CremaReader::itable& table)
    {
        this->Child_leaflet = new Table_relentChild_leafletTable(table.dataset().tables()["Table_relent.Child_leaflet"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_leaflet->Rows, Table_relentSetChild_leaflet);
    }
    
    Table_relentTable::~Table_relentTable()
    {
        delete this->Child_leaflet;
    }
    
    void* Table_relentTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_relentRow(row, ((Table_relentTable*)(table)));
    }
    
    const Table_relentRow* Table_relentTable::Find(unsigned short adviser) const
    {
        return this->FindRow(adviser);
    }
    
    Table_chromatographyChild_admissionRow::Table_chromatographyChild_admissionRow(CremaReader::irow& row, Table_chromatographyChild_admissionTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Shackleton = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->austereness = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->casaba = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->reflection = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->scruffily = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->hierarchical = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->intentional = ((Type3)(row.to_int32(6)));
        }
        this->SetKey(this->Shackleton);
    }
    
    Table_chromatographyChild_admissionTable::Table_chromatographyChild_admissionTable()
    {
    }
    
    Table_chromatographyChild_admissionTable::Table_chromatographyChild_admissionTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_chromatographyChild_admissionTable::Table_chromatographyChild_admissionTable(std::vector<Table_chromatographyChild_admissionRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_chromatographyChild_admissionTable::~Table_chromatographyChild_admissionTable()
    {
    }
    
    void* Table_chromatographyChild_admissionTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_chromatographyChild_admissionRow(row, ((Table_chromatographyChild_admissionTable*)(table)));
    }
    
    const Table_chromatographyChild_admissionRow* Table_chromatographyChild_admissionTable::Find(unsigned long long Shackleton) const
    {
        return this->FindRow(Shackleton);
    }
    
    Table_chromatographyChild1Row::Table_chromatographyChild1Row(CremaReader::irow& row, Table_chromatographyChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hamster = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->badinage = ((Type3)(row.to_int32(1)));
        }
        this->misogamist = ((Type33)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->anathema = row.to_uint8(3);
        }
        if (row.has_value(4))
        {
            this->Armin = row.to_int16(4);
        }
        if (row.has_value(5))
        {
            this->syllabusss = row.to_datetime(5);
        }
        this->SetKey(this->hamster, this->misogamist);
    }
    
    Table_chromatographyChild1Table::Table_chromatographyChild1Table()
    {
    }
    
    Table_chromatographyChild1Table::Table_chromatographyChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_chromatographyChild1Table::Table_chromatographyChild1Table(std::vector<Table_chromatographyChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_chromatographyChild1Table::~Table_chromatographyChild1Table()
    {
    }
    
    void* Table_chromatographyChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_chromatographyChild1Row(row, ((Table_chromatographyChild1Table*)(table)));
    }
    
    const Table_chromatographyChild1Row* Table_chromatographyChild1Table::Find(unsigned char hamster, Type33 misogamist) const
    {
        return this->FindRow(hamster, misogamist);
    }
    
    Table_chromatographyChild_admissionTable Table_chromatographyRow::Child_admissionEmpty;
    
    Table_chromatographyChild1Table Table_chromatographyRow::Child1Empty;
    
    Table_chromatographyRow::Table_chromatographyRow(CremaReader::irow& row, Table_chromatographyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Stendhal = row.to_datetime(0);
        this->cheerer = row.to_int32(1);
        if (row.has_value(2))
        {
            this->wreckage = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Andriette = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->oviduct = row.to_int64(4);
        }
        if ((this->Child_admission == nullptr))
        {
            this->Child_admission = &(Table_chromatographyRow::Child_admissionEmpty);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_chromatographyRow::Child1Empty);
        }
        this->SetKey(this->Stendhal, this->cheerer);
    }
    
    void Table_chromatographySetChild_admission(Table_chromatographyRow* target, const std::vector<Table_chromatographyChild_admissionRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_admission = new Table_chromatographyChild_admissionTable(childs);
    }
    
    void Table_chromatographySetChild1(Table_chromatographyRow* target, const std::vector<Table_chromatographyChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_chromatographyChild1Table(childs);
    }
    
    Table_chromatographyTable::Table_chromatographyTable()
    {
    }
    
    Table_chromatographyTable::Table_chromatographyTable(CremaReader::itable& table)
    {
        this->Child_admission = new Table_chromatographyChild_admissionTable(table.dataset().tables()["Table_chromatography.Child_admission"]);
        this->Child1 = new Table_chromatographyChild1Table(table.dataset().tables()["Table_chromatography.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_admission->Rows, Table_chromatographySetChild_admission);
        this->SetRelations(this->Child1->Rows, Table_chromatographySetChild1);
    }
    
    Table_chromatographyTable::~Table_chromatographyTable()
    {
        delete this->Child_admission;
        delete this->Child1;
    }
    
    void* Table_chromatographyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_chromatographyRow(row, ((Table_chromatographyTable*)(table)));
    }
    
    const Table_chromatographyRow* Table_chromatographyTable::Find(time_t Stendhal, int cheerer) const
    {
        return this->FindRow(Stendhal, cheerer);
    }
    
    Table_CPIChild1Row::Table_CPIChild1Row(CremaReader::irow& row, Table_CPIChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Patti = row.to_duration(0);
        if (row.has_value(1))
        {
            this->glazed = row.to_double(1);
        }
        this->diffract = row.to_boolean(2);
        if (row.has_value(3))
        {
            this->calamitous = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->Cranmer = row.to_int32(4);
        }
        if (row.has_value(5))
        {
            this->Volvo = row.to_duration(5);
        }
        this->SetKey(this->Patti, this->diffract);
    }
    
    Table_CPIChild1Table::Table_CPIChild1Table()
    {
    }
    
    Table_CPIChild1Table::Table_CPIChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_CPIChild1Table::Table_CPIChild1Table(std::vector<Table_CPIChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_CPIChild1Table::~Table_CPIChild1Table()
    {
    }
    
    void* Table_CPIChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_CPIChild1Row(row, ((Table_CPIChild1Table*)(table)));
    }
    
    const Table_CPIChild1Row* Table_CPIChild1Table::Find(int Patti, bool diffract) const
    {
        return this->FindRow(Patti, diffract);
    }
    
    Table_CPIChild1Table Table_CPIRow::Child1Empty;
    
    Table_CPIRow::Table_CPIRow(CremaReader::irow& row, Table_CPITable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->snapback = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->caseworker = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Adonis = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->borderer = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->trollish = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->lukewarm = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->South = row.to_uint8(6);
        }
        if (row.has_value(7))
        {
            this->wagging = row.to_uint32(7);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_CPIRow::Child1Empty);
        }
        this->SetKey(this->snapback);
    }
    
    void Table_CPISetChild1(Table_CPIRow* target, const std::vector<Table_CPIChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_CPIChild1Table(childs);
    }
    
    Table_CPITable::Table_CPITable()
    {
    }
    
    Table_CPITable::Table_CPITable(CremaReader::itable& table)
    {
        this->Child1 = new Table_CPIChild1Table(table.dataset().tables()["Table_CPI.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_CPISetChild1);
    }
    
    Table_CPITable::~Table_CPITable()
    {
        delete this->Child1;
    }
    
    void* Table_CPITable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_CPIRow(row, ((Table_CPITable*)(table)));
    }
    
    const Table_CPIRow* Table_CPITable::Find(unsigned int snapback) const
    {
        return this->FindRow(snapback);
    }
    
    Table_metempsychosesChild1Row::Table_metempsychosesChild1Row(CremaReader::irow& row, Table_metempsychosesChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->parabolic = row.to_int32(0);
        if (row.has_value(1))
        {
            this->syncopation = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->steamroller = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->purloiner = row.to_int64(3);
        }
        this->membership = row.to_int16(4);
        if (row.has_value(5))
        {
            this->Redondo = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->peach = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->asbestos = row.to_int64(7);
        }
        if (row.has_value(8))
        {
            this->rectory = row.to_uint16(8);
        }
        if (row.has_value(9))
        {
            this->printable = row.to_uint64(9);
        }
        if (row.has_value(10))
        {
            this->synapse = ((Type_housebreaking)(row.to_int32(10)));
        }
        this->SetKey(this->parabolic, this->membership);
    }
    
    Table_metempsychosesChild1Table::Table_metempsychosesChild1Table()
    {
    }
    
    Table_metempsychosesChild1Table::Table_metempsychosesChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_metempsychosesChild1Table::Table_metempsychosesChild1Table(std::vector<Table_metempsychosesChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_metempsychosesChild1Table::~Table_metempsychosesChild1Table()
    {
    }
    
    void* Table_metempsychosesChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_metempsychosesChild1Row(row, ((Table_metempsychosesChild1Table*)(table)));
    }
    
    const Table_metempsychosesChild1Row* Table_metempsychosesChild1Table::Find(int parabolic, short membership) const
    {
        return this->FindRow(parabolic, membership);
    }
    
    Table_metempsychosesChild1Table Table_metempsychosesRow::Child1Empty;
    
    Table_metempsychosesRow::Table_metempsychosesRow(CremaReader::irow& row, Table_metempsychosesTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->marginalia = row.to_uint64(0);
        this->chatted = row.to_int64(1);
        if (row.has_value(2))
        {
            this->dutiful = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->Rozella = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->rewedding = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->brunet = row.to_uint16(5);
        }
        if (row.has_value(6))
        {
            this->sweeping = row.to_int16(6);
        }
        if (row.has_value(7))
        {
            this->priesthood = row.to_int32(7);
        }
        if (row.has_value(8))
        {
            this->snatch = row.to_int16(8);
        }
        if (row.has_value(9))
        {
            this->highhandedness = row.to_uint8(9);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_metempsychosesRow::Child1Empty);
        }
        this->SetKey(this->marginalia, this->chatted);
    }
    
    void Table_metempsychosesSetChild1(Table_metempsychosesRow* target, const std::vector<Table_metempsychosesChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_metempsychosesChild1Table(childs);
    }
    
    Table_metempsychosesTable::Table_metempsychosesTable()
    {
    }
    
    Table_metempsychosesTable::Table_metempsychosesTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_metempsychosesChild1Table(table.dataset().tables()["Table_metempsychoses.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_metempsychosesSetChild1);
    }
    
    Table_metempsychosesTable::~Table_metempsychosesTable()
    {
        delete this->Child1;
    }
    
    void* Table_metempsychosesTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_metempsychosesRow(row, ((Table_metempsychosesTable*)(table)));
    }
    
    const Table_metempsychosesRow* Table_metempsychosesTable::Find(unsigned long long marginalia, long long chatted) const
    {
        return this->FindRow(marginalia, chatted);
    }
    
    Table122Row::Table122Row(CremaReader::irow& row, Table122Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->millipede = ((Type8)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Lucilia = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->conjecturer = row.to_uint32(2);
        }
        this->emergent = ((Type21)(row.to_int32(3)));
        if (row.has_value(4))
        {
            this->preserver = row.to_int32(4);
        }
        this->SetKey(this->millipede, this->emergent);
    }
    
    Table122Table::Table122Table()
    {
    }
    
    Table122Table::Table122Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table122Table::~Table122Table()
    {
    }
    
    void* Table122Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table122Row(row, ((Table122Table*)(table)));
    }
    
    const Table122Row* Table122Table::Find(Type8 millipede, Type21 emergent) const
    {
        return this->FindRow(millipede, emergent);
    }
    
    Table134Row::Table134Row(CremaReader::irow& row, Table134Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lumberyard = ((Type6)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Carl = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->firearm = row.to_boolean(2);
        }
        this->uncap = row.to_int8(3);
        if (row.has_value(4))
        {
            this->chairwoman = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->Bassett = ((Type8)(row.to_int32(5)));
        }
        this->SetKey(this->lumberyard, this->uncap);
    }
    
    Table134Table::Table134Table()
    {
    }
    
    Table134Table::Table134Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table134Table::~Table134Table()
    {
    }
    
    void* Table134Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table134Row(row, ((Table134Table*)(table)));
    }
    
    const Table134Row* Table134Table::Find(Type6 lumberyard, char uncap) const
    {
        return this->FindRow(lumberyard, uncap);
    }
    
    Table25Row::Table25Row(CremaReader::irow& row, Table25Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->payed = row.to_int16(0);
        if (row.has_value(1))
        {
            this->Neville = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->loom = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->laxativeness = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->exploitation = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->stencil = row.to_uint64(5);
        }
        this->SetKey(this->payed);
    }
    
    Table25Table::Table25Table()
    {
    }
    
    Table25Table::Table25Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table25Table::~Table25Table()
    {
    }
    
    void* Table25Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table25Row(row, ((Table25Table*)(table)));
    }
    
    const Table25Row* Table25Table::Find(short payed) const
    {
        return this->FindRow(payed);
    }
    
    Table73Row::Table73Row(CremaReader::irow& row, Table73Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->wicking = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Kerstin = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->Baudoin = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->Fitzroy = row.to_double(3);
        }
        if (row.has_value(4))
        {
            this->Macmillan = row.to_int8(4);
        }
        this->Mannie = row.to_int16(5);
        if (row.has_value(6))
        {
            this->dotage = row.to_boolean(6);
        }
        if (row.has_value(7))
        {
            this->Asturias = row.to_int32(7);
        }
        if (row.has_value(8))
        {
            this->milliard = row.to_datetime(8);
        }
        if (row.has_value(9))
        {
            this->congregational = row.to_int16(9);
        }
        this->SetKey(this->wicking, this->Mannie);
    }
    
    Table73Table::Table73Table()
    {
    }
    
    Table73Table::Table73Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table73Table::~Table73Table()
    {
    }
    
    void* Table73Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table73Row(row, ((Table73Table*)(table)));
    }
    
    const Table73Row* Table73Table::Find(char wicking, short Mannie) const
    {
        return this->FindRow(wicking, Mannie);
    }
    
    Table_oxidizesChild1Row::Table_oxidizesChild1Row(CremaReader::irow& row, Table_oxidizesChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->woodlouse = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Letitia = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->Livonia = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->Praia = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->Christmas = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->earner = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->forwent = row.to_duration(6);
        }
        if (row.has_value(7))
        {
            this->dependability = row.to_uint32(7);
        }
        this->SetKey(this->woodlouse);
    }
    
    Table_oxidizesChild1Table::Table_oxidizesChild1Table()
    {
    }
    
    Table_oxidizesChild1Table::Table_oxidizesChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_oxidizesChild1Table::Table_oxidizesChild1Table(std::vector<Table_oxidizesChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_oxidizesChild1Table::~Table_oxidizesChild1Table()
    {
    }
    
    void* Table_oxidizesChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_oxidizesChild1Row(row, ((Table_oxidizesChild1Table*)(table)));
    }
    
    const Table_oxidizesChild1Row* Table_oxidizesChild1Table::Find(unsigned short woodlouse) const
    {
        return this->FindRow(woodlouse);
    }
    
    Table_oxidizesChild2Row::Table_oxidizesChild2Row(CremaReader::irow& row, Table_oxidizesChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Lynn = row.to_int8(0);
        if (row.has_value(1))
        {
            this->clause = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->scrupulosity = row.to_int16(2);
        }
        this->abbrev = row.to_uint16(3);
        if (row.has_value(4))
        {
            this->microdot = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->Estes = ((Type_Multan)(row.to_int32(5)));
        }
        this->SetKey(this->Lynn, this->abbrev);
    }
    
    Table_oxidizesChild2Table::Table_oxidizesChild2Table()
    {
    }
    
    Table_oxidizesChild2Table::Table_oxidizesChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_oxidizesChild2Table::Table_oxidizesChild2Table(std::vector<Table_oxidizesChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_oxidizesChild2Table::~Table_oxidizesChild2Table()
    {
    }
    
    void* Table_oxidizesChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_oxidizesChild2Row(row, ((Table_oxidizesChild2Table*)(table)));
    }
    
    const Table_oxidizesChild2Row* Table_oxidizesChild2Table::Find(char Lynn, unsigned short abbrev) const
    {
        return this->FindRow(Lynn, abbrev);
    }
    
    Table_oxidizesChild_tapiocaRow::Table_oxidizesChild_tapiocaRow(CremaReader::irow& row, Table_oxidizesChild_tapiocaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->illegality = row.to_uint8(0);
        this->metricize = row.to_uint32(1);
        if (row.has_value(2))
        {
            this->bong = row.to_string(2);
        }
        this->confound = row.to_uint32(3);
        if (row.has_value(4))
        {
            this->coprophagous = row.to_boolean(4);
        }
        this->rosin = row.to_uint32(5);
        this->SetKey(this->illegality, this->metricize, this->confound, this->rosin);
    }
    
    Table_oxidizesChild_tapiocaTable::Table_oxidizesChild_tapiocaTable()
    {
    }
    
    Table_oxidizesChild_tapiocaTable::Table_oxidizesChild_tapiocaTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_oxidizesChild_tapiocaTable::Table_oxidizesChild_tapiocaTable(std::vector<Table_oxidizesChild_tapiocaRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_oxidizesChild_tapiocaTable::~Table_oxidizesChild_tapiocaTable()
    {
    }
    
    void* Table_oxidizesChild_tapiocaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_oxidizesChild_tapiocaRow(row, ((Table_oxidizesChild_tapiocaTable*)(table)));
    }
    
    const Table_oxidizesChild_tapiocaRow* Table_oxidizesChild_tapiocaTable::Find(unsigned char illegality, unsigned int metricize, unsigned int confound, unsigned int rosin) const
    {
        return this->FindRow(illegality, metricize, confound, rosin);
    }
    
    Table_oxidizesChild1Table Table_oxidizesRow::Child1Empty;
    
    Table_oxidizesChild2Table Table_oxidizesRow::Child2Empty;
    
    Table_oxidizesChild_tapiocaTable Table_oxidizesRow::Child_tapiocaEmpty;
    
    Table_oxidizesRow::Table_oxidizesRow(CremaReader::irow& row, Table_oxidizesTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->multiplicative = ((Type_Multan)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->math = ((Type_Meiji)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->rickshaw = ((Type_Meiji)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->dimmed = ((Type_Arlan)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->bucker = row.to_double(4);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_oxidizesRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_oxidizesRow::Child2Empty);
        }
        if ((this->Child_tapioca == nullptr))
        {
            this->Child_tapioca = &(Table_oxidizesRow::Child_tapiocaEmpty);
        }
        this->SetKey(this->multiplicative);
    }
    
    void Table_oxidizesSetChild1(Table_oxidizesRow* target, const std::vector<Table_oxidizesChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_oxidizesChild1Table(childs);
    }
    
    void Table_oxidizesSetChild2(Table_oxidizesRow* target, const std::vector<Table_oxidizesChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_oxidizesChild2Table(childs);
    }
    
    void Table_oxidizesSetChild_tapioca(Table_oxidizesRow* target, const std::vector<Table_oxidizesChild_tapiocaRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_tapioca = new Table_oxidizesChild_tapiocaTable(childs);
    }
    
    Table_oxidizesTable::Table_oxidizesTable()
    {
    }
    
    Table_oxidizesTable::Table_oxidizesTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_oxidizesChild1Table(table.dataset().tables()["Table_oxidizes.Child1"]);
        this->Child2 = new Table_oxidizesChild2Table(table.dataset().tables()["Table_oxidizes.Child2"]);
        this->Child_tapioca = new Table_oxidizesChild_tapiocaTable(table.dataset().tables()["Table_oxidizes.Child_tapioca"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_oxidizesSetChild1);
        this->SetRelations(this->Child2->Rows, Table_oxidizesSetChild2);
        this->SetRelations(this->Child_tapioca->Rows, Table_oxidizesSetChild_tapioca);
    }
    
    Table_oxidizesTable::~Table_oxidizesTable()
    {
        delete this->Child1;
        delete this->Child2;
        delete this->Child_tapioca;
    }
    
    void* Table_oxidizesTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_oxidizesRow(row, ((Table_oxidizesTable*)(table)));
    }
    
    const Table_oxidizesRow* Table_oxidizesTable::Find(Type_Multan multiplicative) const
    {
        return this->FindRow(multiplicative);
    }
    
    Table_KerouacRow::Table_KerouacRow(CremaReader::irow& row, Table_KerouacTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Sinclare = ((Type_applejack)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->theosophical = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Edith = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->Drugi = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->bemused = row.to_duration(4);
        }
        if (row.has_value(5))
        {
            this->Tobie = row.to_int16(5);
        }
        this->masculine = row.to_uint64(6);
        if (row.has_value(7))
        {
            this->briefed = ((Type55)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->juxtaposition = ((Type_insolent)(row.to_int32(8)));
        }
        if (row.has_value(9))
        {
            this->pater = row.to_boolean(9);
        }
        if (row.has_value(10))
        {
            this->Nigeria = row.to_uint64(10);
        }
        if (row.has_value(11))
        {
            this->scooper = row.to_uint8(11);
        }
        this->SetKey(this->Sinclare, this->masculine);
    }
    
    Table_KerouacTable::Table_KerouacTable()
    {
    }
    
    Table_KerouacTable::Table_KerouacTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_KerouacTable::~Table_KerouacTable()
    {
    }
    
    void* Table_KerouacTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_KerouacRow(row, ((Table_KerouacTable*)(table)));
    }
    
    const Table_KerouacRow* Table_KerouacTable::Find(Type_applejack Sinclare, unsigned long long masculine) const
    {
        return this->FindRow(Sinclare, masculine);
    }
    
    Table13Row::Table13Row(CremaReader::irow& row, Table13Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->spoiled = row.to_single(0);
        if (row.has_value(1))
        {
            this->serviles = ((Type_Arlan)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->nemesis = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->Hamil = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->Theodora = ((Type_Meiji)(row.to_int32(4)));
        }
        this->SetKey(this->spoiled);
    }
    
    Table13Table::Table13Table()
    {
    }
    
    Table13Table::Table13Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table13Table::~Table13Table()
    {
    }
    
    void* Table13Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table13Row(row, ((Table13Table*)(table)));
    }
    
    const Table13Row* Table13Table::Find(float spoiled) const
    {
        return this->FindRow(spoiled);
    }
    
    Table_gangliaChild1Row::Table_gangliaChild1Row(CremaReader::irow& row, Table_gangliaChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->schism = row.to_int64(0);
        if (row.has_value(1))
        {
            this->BBC = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->Ge = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->grubbed = row.to_int8(3);
        }
        if (row.has_value(4))
        {
            this->sturdiness = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->Lombardy = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->Dredi = row.to_string(6);
        }
        if (row.has_value(7))
        {
            this->sportsmanship = row.to_int16(7);
        }
        this->SetKey(this->schism);
    }
    
    Table_gangliaChild1Table::Table_gangliaChild1Table()
    {
    }
    
    Table_gangliaChild1Table::Table_gangliaChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_gangliaChild1Table::Table_gangliaChild1Table(std::vector<Table_gangliaChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_gangliaChild1Table::~Table_gangliaChild1Table()
    {
    }
    
    void* Table_gangliaChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_gangliaChild1Row(row, ((Table_gangliaChild1Table*)(table)));
    }
    
    const Table_gangliaChild1Row* Table_gangliaChild1Table::Find(long long schism) const
    {
        return this->FindRow(schism);
    }
    
    Table_gangliaChild_frostbiteRow::Table_gangliaChild_frostbiteRow(CremaReader::irow& row, Table_gangliaChild_frostbiteTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->gos = row.to_int8(0);
        this->SetKey(this->gos);
    }
    
    Table_gangliaChild_frostbiteTable::Table_gangliaChild_frostbiteTable()
    {
    }
    
    Table_gangliaChild_frostbiteTable::Table_gangliaChild_frostbiteTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_gangliaChild_frostbiteTable::Table_gangliaChild_frostbiteTable(std::vector<Table_gangliaChild_frostbiteRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_gangliaChild_frostbiteTable::~Table_gangliaChild_frostbiteTable()
    {
    }
    
    void* Table_gangliaChild_frostbiteTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_gangliaChild_frostbiteRow(row, ((Table_gangliaChild_frostbiteTable*)(table)));
    }
    
    const Table_gangliaChild_frostbiteRow* Table_gangliaChild_frostbiteTable::Find(char gos) const
    {
        return this->FindRow(gos);
    }
    
    Table_gangliaChild1Table Table_gangliaRow::Child1Empty;
    
    Table_gangliaChild_frostbiteTable Table_gangliaRow::Child_frostbiteEmpty;
    
    Table_gangliaRow::Table_gangliaRow(CremaReader::irow& row, Table_gangliaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->reprehensible = ((Type3)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->stealthy = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->unlucky = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->Delhi = row.to_uint64(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_gangliaRow::Child1Empty);
        }
        if ((this->Child_frostbite == nullptr))
        {
            this->Child_frostbite = &(Table_gangliaRow::Child_frostbiteEmpty);
        }
        this->SetKey(this->reprehensible);
    }
    
    void Table_gangliaSetChild1(Table_gangliaRow* target, const std::vector<Table_gangliaChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_gangliaChild1Table(childs);
    }
    
    void Table_gangliaSetChild_frostbite(Table_gangliaRow* target, const std::vector<Table_gangliaChild_frostbiteRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_frostbite = new Table_gangliaChild_frostbiteTable(childs);
    }
    
    Table_gangliaTable::Table_gangliaTable()
    {
    }
    
    Table_gangliaTable::Table_gangliaTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_gangliaChild1Table(table.dataset().tables()["Table_ganglia.Child1"]);
        this->Child_frostbite = new Table_gangliaChild_frostbiteTable(table.dataset().tables()["Table_ganglia.Child_frostbite"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_gangliaSetChild1);
        this->SetRelations(this->Child_frostbite->Rows, Table_gangliaSetChild_frostbite);
    }
    
    Table_gangliaTable::~Table_gangliaTable()
    {
        delete this->Child1;
        delete this->Child_frostbite;
    }
    
    void* Table_gangliaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_gangliaRow(row, ((Table_gangliaTable*)(table)));
    }
    
    const Table_gangliaRow* Table_gangliaTable::Find(Type3 reprehensible) const
    {
        return this->FindRow(reprehensible);
    }
    
    Table_approveChild1Row::Table_approveChild1Row(CremaReader::irow& row, Table_approveChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->parabolic = row.to_int32(0);
        if (row.has_value(1))
        {
            this->syncopation = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->steamroller = row.to_boolean(2);
        }
        this->rattrap = row.to_int16(3);
        if (row.has_value(4))
        {
            this->Redondo = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->peach = row.to_int8(5);
        }
        this->SetKey(this->parabolic, this->rattrap);
    }
    
    Table_approveChild1Table::Table_approveChild1Table()
    {
    }
    
    Table_approveChild1Table::Table_approveChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_approveChild1Table::Table_approveChild1Table(std::vector<Table_approveChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_approveChild1Table::~Table_approveChild1Table()
    {
    }
    
    void* Table_approveChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_approveChild1Row(row, ((Table_approveChild1Table*)(table)));
    }
    
    const Table_approveChild1Row* Table_approveChild1Table::Find(int parabolic, short rattrap) const
    {
        return this->FindRow(parabolic, rattrap);
    }
    
    Table_approveChild1Table Table_approveRow::Child1Empty;
    
    Table_approveRow::Table_approveRow(CremaReader::irow& row, Table_approveTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->marginalia = row.to_uint64(0);
        this->chatted = row.to_int64(1);
        if (row.has_value(2))
        {
            this->dutiful = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->strangulate = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->jazz = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->Rozella = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->patrician = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->brunet = row.to_uint16(7);
        }
        if (row.has_value(8))
        {
            this->sweeping = row.to_int16(8);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_approveRow::Child1Empty);
        }
        this->SetKey(this->marginalia, this->chatted);
    }
    
    void Table_approveSetChild1(Table_approveRow* target, const std::vector<Table_approveChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_approveChild1Table(childs);
    }
    
    Table_approveTable::Table_approveTable()
    {
    }
    
    Table_approveTable::Table_approveTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_approveChild1Table(table.dataset().tables()["Table_approve.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_approveSetChild1);
    }
    
    Table_approveTable::~Table_approveTable()
    {
        delete this->Child1;
    }
    
    void* Table_approveTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_approveRow(row, ((Table_approveTable*)(table)));
    }
    
    const Table_approveRow* Table_approveTable::Find(unsigned long long marginalia, long long chatted) const
    {
        return this->FindRow(marginalia, chatted);
    }
    
    Table_SCRow::Table_SCRow(CremaReader::irow& row, Table_SCTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->immediacy = row.to_int32(0);
        if (row.has_value(1))
        {
            this->dateline = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->setup = ((Type_Gretta)(row.to_int32(2)));
        }
        this->lettering = row.to_double(3);
        this->colatitude = ((Type_Multan)(row.to_int32(4)));
        if (row.has_value(5))
        {
            this->Sam = row.to_int8(5);
        }
        this->maleficence = row.to_single(6);
        if (row.has_value(7))
        {
            this->Fermi = ((Type1)(row.to_int32(7)));
        }
        this->Evered = ((Type_Madison)(row.to_int32(8)));
        if (row.has_value(9))
        {
            this->isthmian = row.to_int64(9);
        }
        if (row.has_value(10))
        {
            this->callosity = row.to_datetime(10);
        }
        this->SetKey(this->immediacy, this->lettering, this->colatitude, this->maleficence, this->Evered);
    }
    
    Table_SCTable::Table_SCTable()
    {
    }
    
    Table_SCTable::Table_SCTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_SCTable::~Table_SCTable()
    {
    }
    
    void* Table_SCTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_SCRow(row, ((Table_SCTable*)(table)));
    }
    
    const Table_SCRow* Table_SCTable::Find(int immediacy, double lettering, Type_Multan colatitude, float maleficence, Type_Madison Evered) const
    {
        return this->FindRow(immediacy, lettering, colatitude, maleficence, Evered);
    }
    
    Table_ReadeRow::Table_ReadeRow(CremaReader::irow& row, Table_ReadeTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->lumberyard = ((Type6)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Carl = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->firearm = row.to_boolean(2);
        }
        this->uncap = row.to_int8(3);
        if (row.has_value(4))
        {
            this->chairwoman = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->Bassett = ((Type8)(row.to_int32(5)));
        }
        this->SetKey(this->lumberyard, this->uncap);
    }
    
    Table_ReadeTable::Table_ReadeTable()
    {
    }
    
    Table_ReadeTable::Table_ReadeTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_ReadeTable::~Table_ReadeTable()
    {
    }
    
    void* Table_ReadeTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_ReadeRow(row, ((Table_ReadeTable*)(table)));
    }
    
    const Table_ReadeRow* Table_ReadeTable::Find(Type6 lumberyard, char uncap) const
    {
        return this->FindRow(lumberyard, uncap);
    }
    
    Table201Row::Table201Row(CremaReader::irow& row, Table201Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->ultimateness = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->erosive = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->tackler = row.to_duration(2);
        }
        this->Glass = row.to_uint32(3);
        if (row.has_value(4))
        {
            this->amylase = row.to_single(4);
        }
        this->impose = row.to_string(5);
        this->SetKey(this->ultimateness, this->Glass, this->impose);
    }
    
    Table201Table::Table201Table()
    {
    }
    
    Table201Table::Table201Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table201Table::~Table201Table()
    {
    }
    
    void* Table201Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table201Row(row, ((Table201Table*)(table)));
    }
    
    const Table201Row* Table201Table::Find(time_t ultimateness, unsigned int Glass, const std::string& impose) const
    {
        return this->FindRow(ultimateness, Glass, impose);
    }
    
    Table_culpritRow::Table_culpritRow(CremaReader::irow& row, Table_culpritTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Elie = row.to_duration(0);
        if (row.has_value(1))
        {
            this->traction = row.to_int16(1);
        }
        if (row.has_value(2))
        {
            this->cartload = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->Cartwright = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->lemme = row.to_uint16(4);
        }
        if (row.has_value(5))
        {
            this->Timex = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->Zacharia = row.to_boolean(6);
        }
        this->SetKey(this->Elie);
    }
    
    Table_culpritTable::Table_culpritTable()
    {
    }
    
    Table_culpritTable::Table_culpritTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_culpritTable::~Table_culpritTable()
    {
    }
    
    void* Table_culpritTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_culpritRow(row, ((Table_culpritTable*)(table)));
    }
    
    const Table_culpritRow* Table_culpritTable::Find(int Elie) const
    {
        return this->FindRow(Elie);
    }
    
    Table_tsunamiRow::Table_tsunamiRow(CremaReader::irow& row, Table_tsunamiTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->culpableness = row.to_single(0);
        this->SetKey(this->culpableness);
    }
    
    Table_tsunamiTable::Table_tsunamiTable()
    {
    }
    
    Table_tsunamiTable::Table_tsunamiTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_tsunamiTable::~Table_tsunamiTable()
    {
    }
    
    void* Table_tsunamiTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_tsunamiRow(row, ((Table_tsunamiTable*)(table)));
    }
    
    const Table_tsunamiRow* Table_tsunamiTable::Find(float culpableness) const
    {
        return this->FindRow(culpableness);
    }
    
    Table135Row::Table135Row(CremaReader::irow& row, Table135Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->sloughs = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->crackerjack = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->rehearsal = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->Coloradan = row.to_uint8(3);
        }
        if (row.has_value(4))
        {
            this->substrate = row.to_int8(4);
        }
        this->SetKey(this->sloughs);
    }
    
    Table135Table::Table135Table()
    {
    }
    
    Table135Table::Table135Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table135Table::~Table135Table()
    {
    }
    
    void* Table135Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table135Row(row, ((Table135Table*)(table)));
    }
    
    const Table135Row* Table135Table::Find(bool sloughs) const
    {
        return this->FindRow(sloughs);
    }
    
    Table19Row::Table19Row(CremaReader::irow& row, Table19Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->botch = row.to_boolean(0);
        if (row.has_value(1))
        {
            this->tribesman = row.to_uint8(1);
        }
        this->Principe = row.to_datetime(2);
        if (row.has_value(3))
        {
            this->rubbed = row.to_int8(3);
        }
        if (row.has_value(4))
        {
            this->effluvium = ((Type13)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->hardtop = row.to_uint16(5);
        }
        if (row.has_value(6))
        {
            this->Foch = ((Type8)(row.to_int32(6)));
        }
        this->testator = row.to_int64(7);
        if (row.has_value(8))
        {
            this->Corvallis = row.to_int16(8);
        }
        if (row.has_value(9))
        {
            this->lamina = ((Type_Jenelle)(row.to_int32(9)));
        }
        if (row.has_value(10))
        {
            this->thriver = row.to_boolean(10);
        }
        if (row.has_value(11))
        {
            this->sparring = row.to_boolean(11);
        }
        if (row.has_value(12))
        {
            this->psychophysiology = ((Type_spokespeople)(row.to_int32(12)));
        }
        if (row.has_value(13))
        {
            this->breeching = row.to_uint32(13);
        }
        this->SetKey(this->botch, this->Principe, this->testator);
    }
    
    Table19Table::Table19Table()
    {
    }
    
    Table19Table::Table19Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table19Table::~Table19Table()
    {
    }
    
    void* Table19Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table19Row(row, ((Table19Table*)(table)));
    }
    
    const Table19Row* Table19Table::Find(bool botch, time_t Principe, long long testator) const
    {
        return this->FindRow(botch, Principe, testator);
    }
    
    Table37Row::Table37Row(CremaReader::irow& row, Table37Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->culmination = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->whitter = ((Type30)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->Vickie = row.to_int16(2);
        }
        this->SetKey(this->culmination);
    }
    
    Table37Table::Table37Table()
    {
    }
    
    Table37Table::Table37Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table37Table::~Table37Table()
    {
    }
    
    void* Table37Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table37Row(row, ((Table37Table*)(table)));
    }
    
    const Table37Row* Table37Table::Find(unsigned char culmination) const
    {
        return this->FindRow(culmination);
    }
    
    Table_alibiRow::Table_alibiRow(CremaReader::irow& row, Table_alibiTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->misstep = ((Type8)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Alistair = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->birth = ((Type15)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Abdel = row.to_int16(3);
        }
        if (row.has_value(4))
        {
            this->cousinly = row.to_uint64(4);
        }
        if (row.has_value(5))
        {
            this->Paley = row.to_int16(5);
        }
        this->SetKey(this->misstep);
    }
    
    Table_alibiTable::Table_alibiTable()
    {
    }
    
    Table_alibiTable::Table_alibiTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_alibiTable::~Table_alibiTable()
    {
    }
    
    void* Table_alibiTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_alibiRow(row, ((Table_alibiTable*)(table)));
    }
    
    const Table_alibiRow* Table_alibiTable::Find(Type8 misstep) const
    {
        return this->FindRow(misstep);
    }
    
    Table38Row::Table38Row(CremaReader::irow& row, Table38Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->misstep = ((Type8)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Alistair = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->Maximilian = ((Type15)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Abdel = row.to_int16(3);
        }
        if (row.has_value(4))
        {
            this->Paley = row.to_int16(4);
        }
        if (row.has_value(5))
        {
            this->seeker = row.to_boolean(5);
        }
        this->epidermal = row.to_double(6);
        if (row.has_value(7))
        {
            this->musicality = row.to_duration(7);
        }
        if (row.has_value(8))
        {
            this->pointblank = row.to_int16(8);
        }
        if (row.has_value(9))
        {
            this->workplace = row.to_int8(9);
        }
        this->Doroteya = row.to_uint64(10);
        if (row.has_value(11))
        {
            this->HR = row.to_string(11);
        }
        this->SetKey(this->misstep, this->epidermal, this->Doroteya);
    }
    
    Table38Table::Table38Table()
    {
    }
    
    Table38Table::Table38Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table38Table::~Table38Table()
    {
    }
    
    void* Table38Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table38Row(row, ((Table38Table*)(table)));
    }
    
    const Table38Row* Table38Table::Find(Type8 misstep, double epidermal, unsigned long long Doroteya) const
    {
        return this->FindRow(misstep, epidermal, Doroteya);
    }
    
    Table58Child_amoebicRow::Table58Child_amoebicRow(CremaReader::irow& row, Table58Child_amoebicTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->utopia = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->conversationalist = row.to_uint64(1);
        }
        this->scree = row.to_single(2);
        if (row.has_value(3))
        {
            this->farinaceous = ((Type_nephew)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->balderdash = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->extinguishable = row.to_int64(5);
        }
        this->Sieglinda = row.to_duration(6);
        if (row.has_value(7))
        {
            this->switchmen = row.to_uint64(7);
        }
        if (row.has_value(8))
        {
            this->sneering = ((Type_HeraclitusDeletable)(row.to_int32(8)));
        }
        if (row.has_value(9))
        {
            this->Godspeed = ((Type13)(row.to_int32(9)));
        }
        if (row.has_value(10))
        {
            this->Xenophon = row.to_int32(10);
        }
        if (row.has_value(11))
        {
            this->nonporous = row.to_duration(11);
        }
        if (row.has_value(12))
        {
            this->stereoscopic = ((Type_surprise)(row.to_int32(12)));
        }
        this->SetKey(this->utopia, this->scree, this->Sieglinda);
    }
    
    Table58Child_amoebicTable::Table58Child_amoebicTable()
    {
    }
    
    Table58Child_amoebicTable::Table58Child_amoebicTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table58Child_amoebicTable::Table58Child_amoebicTable(std::vector<Table58Child_amoebicRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table58Child_amoebicTable::~Table58Child_amoebicTable()
    {
    }
    
    void* Table58Child_amoebicTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table58Child_amoebicRow(row, ((Table58Child_amoebicTable*)(table)));
    }
    
    const Table58Child_amoebicRow* Table58Child_amoebicTable::Find(unsigned char utopia, float scree, int Sieglinda) const
    {
        return this->FindRow(utopia, scree, Sieglinda);
    }
    
    Table58Child_amoebicTable Table58Row::Child_amoebicEmpty;
    
    Table58Row::Table58Row(CremaReader::irow& row, Table58Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->mango = ((Type_hand)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Isidor = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->relink = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->Martguerita = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->McClain = ((Type_supportedDeletable)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->fixable = row.to_uint16(5);
        }
        if (row.has_value(6))
        {
            this->cookware = row.to_uint64(6);
        }
        if (row.has_value(7))
        {
            this->Datsun = row.to_double(7);
        }
        if (row.has_value(8))
        {
            this->Cluj = row.to_string(8);
        }
        if ((this->Child_amoebic == nullptr))
        {
            this->Child_amoebic = &(Table58Row::Child_amoebicEmpty);
        }
        this->SetKey(this->mango);
    }
    
    void Table58SetChild_amoebic(Table58Row* target, const std::vector<Table58Child_amoebicRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_amoebic = new Table58Child_amoebicTable(childs);
    }
    
    Table58Table::Table58Table()
    {
    }
    
    Table58Table::Table58Table(CremaReader::itable& table)
    {
        this->Child_amoebic = new Table58Child_amoebicTable(table.dataset().tables()["Table58.Child_amoebic"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_amoebic->Rows, Table58SetChild_amoebic);
    }
    
    Table58Table::~Table58Table()
    {
        delete this->Child_amoebic;
    }
    
    void* Table58Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table58Row(row, ((Table58Table*)(table)));
    }
    
    const Table58Row* Table58Table::Find(Type_hand mango) const
    {
        return this->FindRow(mango);
    }
    
    Table_consanguineousChild_YorkeRow::Table_consanguineousChild_YorkeRow(CremaReader::irow& row, Table_consanguineousChild_YorkeTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Cantor = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->Bevan = ((Type_rennet)(row.to_int32(1)));
        }
        this->SetKey(this->Cantor);
    }
    
    Table_consanguineousChild_YorkeTable::Table_consanguineousChild_YorkeTable()
    {
    }
    
    Table_consanguineousChild_YorkeTable::Table_consanguineousChild_YorkeTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_consanguineousChild_YorkeTable::Table_consanguineousChild_YorkeTable(std::vector<Table_consanguineousChild_YorkeRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_consanguineousChild_YorkeTable::~Table_consanguineousChild_YorkeTable()
    {
    }
    
    void* Table_consanguineousChild_YorkeTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_consanguineousChild_YorkeRow(row, ((Table_consanguineousChild_YorkeTable*)(table)));
    }
    
    const Table_consanguineousChild_YorkeRow* Table_consanguineousChild_YorkeTable::Find(unsigned int Cantor) const
    {
        return this->FindRow(Cantor);
    }
    
    Table_consanguineousChild_YorkeTable Table_consanguineousRow::Child_YorkeEmpty;
    
    Table_consanguineousRow::Table_consanguineousRow(CremaReader::irow& row, Table_consanguineousTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Argentina = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->wingding = ((Type_Multan)(row.to_int32(1)));
        }
        if ((this->Child_Yorke == nullptr))
        {
            this->Child_Yorke = &(Table_consanguineousRow::Child_YorkeEmpty);
        }
        this->SetKey(this->Argentina);
    }
    
    void Table_consanguineousSetChild_Yorke(Table_consanguineousRow* target, const std::vector<Table_consanguineousChild_YorkeRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Yorke = new Table_consanguineousChild_YorkeTable(childs);
    }
    
    Table_consanguineousTable::Table_consanguineousTable()
    {
    }
    
    Table_consanguineousTable::Table_consanguineousTable(CremaReader::itable& table)
    {
        this->Child_Yorke = new Table_consanguineousChild_YorkeTable(table.dataset().tables()["Table_consanguineous.Child_Yorke"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Yorke->Rows, Table_consanguineousSetChild_Yorke);
    }
    
    Table_consanguineousTable::~Table_consanguineousTable()
    {
        delete this->Child_Yorke;
    }
    
    void* Table_consanguineousTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_consanguineousRow(row, ((Table_consanguineousTable*)(table)));
    }
    
    const Table_consanguineousRow* Table_consanguineousTable::Find(unsigned int Argentina) const
    {
        return this->FindRow(Argentina);
    }
    
    Table42Row::Table42Row(CremaReader::irow& row, Table42Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->washstand = row.to_single(0);
        if (row.has_value(1))
        {
            this->quagmire = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->kiloton = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->fruitful = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->obliviousness = ((Type_Madison)(row.to_int32(4)));
        }
        this->SetKey(this->washstand);
    }
    
    Table42Table::Table42Table()
    {
    }
    
    Table42Table::Table42Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table42Table::~Table42Table()
    {
    }
    
    void* Table42Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table42Row(row, ((Table42Table*)(table)));
    }
    
    const Table42Row* Table42Table::Find(float washstand) const
    {
        return this->FindRow(washstand);
    }
    
    Table72Row::Table72Row(CremaReader::irow& row, Table72Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Tamma = ((Type25)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->housewives = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->jadeite = row.to_int32(2);
        }
        if (row.has_value(3))
        {
            this->rejoinder = row.to_datetime(3);
        }
        this->SetKey(this->Tamma);
    }
    
    Table72Table::Table72Table()
    {
    }
    
    Table72Table::Table72Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table72Table::~Table72Table()
    {
    }
    
    void* Table72Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table72Row(row, ((Table72Table*)(table)));
    }
    
    const Table72Row* Table72Table::Find(Type25 Tamma) const
    {
        return this->FindRow(Tamma);
    }
    
    Table92Row::Table92Row(CremaReader::irow& row, Table92Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Nickey = row.to_uint64(0);
        this->SetKey(this->Nickey);
    }
    
    Table92Table::Table92Table()
    {
    }
    
    Table92Table::Table92Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table92Table::~Table92Table()
    {
    }
    
    void* Table92Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table92Row(row, ((Table92Table*)(table)));
    }
    
    const Table92Row* Table92Table::Find(unsigned long long Nickey) const
    {
        return this->FindRow(Nickey);
    }
    
    Table99Row::Table99Row(CremaReader::irow& row, Table99Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->ultrastructure = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->general = ((Type56)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->caucus = ((Type80)(row.to_int32(2)));
        }
        this->SetKey(this->ultrastructure);
    }
    
    Table99Table::Table99Table()
    {
    }
    
    Table99Table::Table99Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table99Table::~Table99Table()
    {
    }
    
    void* Table99Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table99Row(row, ((Table99Table*)(table)));
    }
    
    const Table99Row* Table99Table::Find(unsigned long long ultrastructure) const
    {
        return this->FindRow(ultrastructure);
    }
    
    Table_JerryChild_quailRow::Table_JerryChild_quailRow(CremaReader::irow& row, Table_JerryChild_quailTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->forcefulness = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->Inglis = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->seismography = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->faultlessness = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->fiver = row.to_uint16(4);
        }
        this->prevention = row.to_int16(5);
        if (row.has_value(6))
        {
            this->purloiner = row.to_datetime(6);
        }
        if (row.has_value(7))
        {
            this->Ancell = row.to_uint64(7);
        }
        this->SetKey(this->forcefulness, this->prevention);
    }
    
    Table_JerryChild_quailTable::Table_JerryChild_quailTable()
    {
    }
    
    Table_JerryChild_quailTable::Table_JerryChild_quailTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_JerryChild_quailTable::Table_JerryChild_quailTable(std::vector<Table_JerryChild_quailRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_JerryChild_quailTable::~Table_JerryChild_quailTable()
    {
    }
    
    void* Table_JerryChild_quailTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_JerryChild_quailRow(row, ((Table_JerryChild_quailTable*)(table)));
    }
    
    const Table_JerryChild_quailRow* Table_JerryChild_quailTable::Find(time_t forcefulness, short prevention) const
    {
        return this->FindRow(forcefulness, prevention);
    }
    
    Table_JerryChild_quailTable Table_JerryRow::Child_quailEmpty;
    
    Table_JerryRow::Table_JerryRow(CremaReader::irow& row, Table_JerryTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Portsmouth = row.to_int16(0);
        if (row.has_value(1))
        {
            this->bout = ((Type_rennet)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->overexpose = row.to_int64(2);
        }
        if (row.has_value(3))
        {
            this->leotard = row.to_uint16(3);
        }
        if ((this->Child_quail == nullptr))
        {
            this->Child_quail = &(Table_JerryRow::Child_quailEmpty);
        }
        this->SetKey(this->Portsmouth);
    }
    
    void Table_JerrySetChild_quail(Table_JerryRow* target, const std::vector<Table_JerryChild_quailRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_quail = new Table_JerryChild_quailTable(childs);
    }
    
    Table_JerryTable::Table_JerryTable()
    {
    }
    
    Table_JerryTable::Table_JerryTable(CremaReader::itable& table)
    {
        this->Child_quail = new Table_JerryChild_quailTable(table.dataset().tables()["Table_Jerry.Child_quail"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_quail->Rows, Table_JerrySetChild_quail);
    }
    
    Table_JerryTable::~Table_JerryTable()
    {
        delete this->Child_quail;
    }
    
    void* Table_JerryTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_JerryRow(row, ((Table_JerryTable*)(table)));
    }
    
    const Table_JerryRow* Table_JerryTable::Find(short Portsmouth) const
    {
        return this->FindRow(Portsmouth);
    }
    
    Table_LeeuwenhoekChild1Row::Table_LeeuwenhoekChild1Row(CremaReader::irow& row, Table_LeeuwenhoekChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Maggi = row.to_int8(0);
        if (row.has_value(1))
        {
            this->Madonna = row.to_boolean(1);
        }
        this->filed = row.to_string(2);
        if (row.has_value(3))
        {
            this->pill = row.to_boolean(3);
        }
        this->SetKey(this->Maggi, this->filed);
    }
    
    Table_LeeuwenhoekChild1Table::Table_LeeuwenhoekChild1Table()
    {
    }
    
    Table_LeeuwenhoekChild1Table::Table_LeeuwenhoekChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LeeuwenhoekChild1Table::Table_LeeuwenhoekChild1Table(std::vector<Table_LeeuwenhoekChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_LeeuwenhoekChild1Table::~Table_LeeuwenhoekChild1Table()
    {
    }
    
    void* Table_LeeuwenhoekChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LeeuwenhoekChild1Row(row, ((Table_LeeuwenhoekChild1Table*)(table)));
    }
    
    const Table_LeeuwenhoekChild1Row* Table_LeeuwenhoekChild1Table::Find(char Maggi, const std::string& filed) const
    {
        return this->FindRow(Maggi, filed);
    }
    
    Table_LeeuwenhoekChild_SakharovRow::Table_LeeuwenhoekChild_SakharovRow(CremaReader::irow& row, Table_LeeuwenhoekChild_SakharovTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->outgrip = row.to_duration(0);
        if (row.has_value(1))
        {
            this->condominium = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->guiltlessness = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->splash = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->entrapping = row.to_boolean(4);
        }
        if (row.has_value(5))
        {
            this->Hamnet = row.to_datetime(5);
        }
        this->SetKey(this->outgrip);
    }
    
    Table_LeeuwenhoekChild_SakharovTable::Table_LeeuwenhoekChild_SakharovTable()
    {
    }
    
    Table_LeeuwenhoekChild_SakharovTable::Table_LeeuwenhoekChild_SakharovTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LeeuwenhoekChild_SakharovTable::Table_LeeuwenhoekChild_SakharovTable(std::vector<Table_LeeuwenhoekChild_SakharovRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_LeeuwenhoekChild_SakharovTable::~Table_LeeuwenhoekChild_SakharovTable()
    {
    }
    
    void* Table_LeeuwenhoekChild_SakharovTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LeeuwenhoekChild_SakharovRow(row, ((Table_LeeuwenhoekChild_SakharovTable*)(table)));
    }
    
    const Table_LeeuwenhoekChild_SakharovRow* Table_LeeuwenhoekChild_SakharovTable::Find(int outgrip) const
    {
        return this->FindRow(outgrip);
    }
    
    Table_LeeuwenhoekChild_nevusRow::Table_LeeuwenhoekChild_nevusRow(CremaReader::irow& row, Table_LeeuwenhoekChild_nevusTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->repetition = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->phonetician = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->Nanni = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->monographs = row.to_boolean(3);
        }
        if (row.has_value(4))
        {
            this->humus = row.to_int64(4);
        }
        if (row.has_value(5))
        {
            this->forswore = ((Type1)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->compatibly = row.to_datetime(6);
        }
        if (row.has_value(7))
        {
            this->Elyn = row.to_int32(7);
        }
        this->SetKey(this->repetition);
    }
    
    Table_LeeuwenhoekChild_nevusTable::Table_LeeuwenhoekChild_nevusTable()
    {
    }
    
    Table_LeeuwenhoekChild_nevusTable::Table_LeeuwenhoekChild_nevusTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_LeeuwenhoekChild_nevusTable::Table_LeeuwenhoekChild_nevusTable(std::vector<Table_LeeuwenhoekChild_nevusRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_LeeuwenhoekChild_nevusTable::~Table_LeeuwenhoekChild_nevusTable()
    {
    }
    
    void* Table_LeeuwenhoekChild_nevusTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LeeuwenhoekChild_nevusRow(row, ((Table_LeeuwenhoekChild_nevusTable*)(table)));
    }
    
    const Table_LeeuwenhoekChild_nevusRow* Table_LeeuwenhoekChild_nevusTable::Find(unsigned char repetition) const
    {
        return this->FindRow(repetition);
    }
    
    Table_LeeuwenhoekChild1Table Table_LeeuwenhoekRow::Child1Empty;
    
    Table_LeeuwenhoekChild_SakharovTable Table_LeeuwenhoekRow::Child_SakharovEmpty;
    
    Table_LeeuwenhoekChild_nevusTable Table_LeeuwenhoekRow::Child_nevusEmpty;
    
    Table_LeeuwenhoekRow::Table_LeeuwenhoekRow(CremaReader::irow& row, Table_LeeuwenhoekTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->eviction = row.to_double(0);
        if (row.has_value(1))
        {
            this->Stefan = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Cesarean = ((Type_Madison)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->operetta = row.to_int64(3);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_LeeuwenhoekRow::Child1Empty);
        }
        if ((this->Child_Sakharov == nullptr))
        {
            this->Child_Sakharov = &(Table_LeeuwenhoekRow::Child_SakharovEmpty);
        }
        if ((this->Child_nevus == nullptr))
        {
            this->Child_nevus = &(Table_LeeuwenhoekRow::Child_nevusEmpty);
        }
        this->SetKey(this->eviction);
    }
    
    void Table_LeeuwenhoekSetChild1(Table_LeeuwenhoekRow* target, const std::vector<Table_LeeuwenhoekChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_LeeuwenhoekChild1Table(childs);
    }
    
    void Table_LeeuwenhoekSetChild_Sakharov(Table_LeeuwenhoekRow* target, const std::vector<Table_LeeuwenhoekChild_SakharovRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Sakharov = new Table_LeeuwenhoekChild_SakharovTable(childs);
    }
    
    void Table_LeeuwenhoekSetChild_nevus(Table_LeeuwenhoekRow* target, const std::vector<Table_LeeuwenhoekChild_nevusRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_nevus = new Table_LeeuwenhoekChild_nevusTable(childs);
    }
    
    Table_LeeuwenhoekTable::Table_LeeuwenhoekTable()
    {
    }
    
    Table_LeeuwenhoekTable::Table_LeeuwenhoekTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_LeeuwenhoekChild1Table(table.dataset().tables()["Table_Leeuwenhoek.Child1"]);
        this->Child_Sakharov = new Table_LeeuwenhoekChild_SakharovTable(table.dataset().tables()["Table_Leeuwenhoek.Child_Sakharov"]);
        this->Child_nevus = new Table_LeeuwenhoekChild_nevusTable(table.dataset().tables()["Table_Leeuwenhoek.Child_nevus"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_LeeuwenhoekSetChild1);
        this->SetRelations(this->Child_Sakharov->Rows, Table_LeeuwenhoekSetChild_Sakharov);
        this->SetRelations(this->Child_nevus->Rows, Table_LeeuwenhoekSetChild_nevus);
    }
    
    Table_LeeuwenhoekTable::~Table_LeeuwenhoekTable()
    {
        delete this->Child1;
        delete this->Child_Sakharov;
        delete this->Child_nevus;
    }
    
    void* Table_LeeuwenhoekTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_LeeuwenhoekRow(row, ((Table_LeeuwenhoekTable*)(table)));
    }
    
    const Table_LeeuwenhoekRow* Table_LeeuwenhoekTable::Find(double eviction) const
    {
        return this->FindRow(eviction);
    }
    
    Table_pertainRow::Table_pertainRow(CremaReader::irow& row, Table_pertainTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Elicia = row.to_int8(0);
        if (row.has_value(1))
        {
            this->execrably = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->Knudsen = ((Type_rennet)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->bulky = ((Type15)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->fake = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->scintillation = row.to_single(5);
        }
        this->SetKey(this->Elicia);
    }
    
    Table_pertainTable::Table_pertainTable()
    {
    }
    
    Table_pertainTable::Table_pertainTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_pertainTable::~Table_pertainTable()
    {
    }
    
    void* Table_pertainTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_pertainRow(row, ((Table_pertainTable*)(table)));
    }
    
    const Table_pertainRow* Table_pertainTable::Find(char Elicia) const
    {
        return this->FindRow(Elicia);
    }
    
    Table163Row::Table163Row(CremaReader::irow& row, Table163Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Defoe = ((Type33)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->passmark = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->glower = row.to_int32(2);
        }
        this->SetKey(this->Defoe);
    }
    
    Table163Table::Table163Table()
    {
    }
    
    Table163Table::Table163Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table163Table::~Table163Table()
    {
    }
    
    void* Table163Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table163Row(row, ((Table163Table*)(table)));
    }
    
    const Table163Row* Table163Table::Find(Type33 Defoe) const
    {
        return this->FindRow(Defoe);
    }
    
    Table174Row::Table174Row(CremaReader::irow& row, Table174Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->apiarist = ((Type45)(row.to_int32(0)));
        this->SetKey(this->apiarist);
    }
    
    Table174Table::Table174Table()
    {
    }
    
    Table174Table::Table174Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table174Table::~Table174Table()
    {
    }
    
    void* Table174Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table174Row(row, ((Table174Table*)(table)));
    }
    
    const Table174Row* Table174Table::Find(Type45 apiarist) const
    {
        return this->FindRow(apiarist);
    }
    
    Table188Row::Table188Row(CremaReader::irow& row, Table188Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Carla = row.to_single(0);
        if (row.has_value(1))
        {
            this->Caspar = row.to_int32(1);
        }
        this->SetKey(this->Carla);
    }
    
    Table188Table::Table188Table()
    {
    }
    
    Table188Table::Table188Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table188Table::~Table188Table()
    {
    }
    
    void* Table188Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table188Row(row, ((Table188Table*)(table)));
    }
    
    const Table188Row* Table188Table::Find(float Carla) const
    {
        return this->FindRow(Carla);
    }
    
    Table_globetrotterRow::Table_globetrotterRow(CremaReader::irow& row, Table_globetrotterTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->duplicative = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->Barkley = row.to_duration(1);
        }
        this->SetKey(this->duplicative);
    }
    
    Table_globetrotterTable::Table_globetrotterTable()
    {
    }
    
    Table_globetrotterTable::Table_globetrotterTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_globetrotterTable::~Table_globetrotterTable()
    {
    }
    
    void* Table_globetrotterTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_globetrotterRow(row, ((Table_globetrotterTable*)(table)));
    }
    
    const Table_globetrotterRow* Table_globetrotterTable::Find(unsigned char duplicative) const
    {
        return this->FindRow(duplicative);
    }
    
    Table173Row::Table173Row(CremaReader::irow& row, Table173Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Rachael = row.to_datetime(0);
        this->SetKey(this->Rachael);
    }
    
    Table173Table::Table173Table()
    {
    }
    
    Table173Table::Table173Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table173Table::~Table173Table()
    {
    }
    
    void* Table173Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table173Row(row, ((Table173Table*)(table)));
    }
    
    const Table173Row* Table173Table::Find(time_t Rachael) const
    {
        return this->FindRow(Rachael);
    }
    
    Table187Row::Table187Row(CremaReader::irow& row, Table187Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->clement = ((Type_Gretta)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->plinker = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->Jay = row.to_duration(2);
        }
        if (row.has_value(3))
        {
            this->Godthaab = row.to_uint32(3);
        }
        if (row.has_value(4))
        {
            this->brucellosis = row.to_uint16(4);
        }
        this->gustily = row.to_datetime(5);
        if (row.has_value(6))
        {
            this->nonplussing = row.to_uint8(6);
        }
        if (row.has_value(7))
        {
            this->MacLeish = row.to_double(7);
        }
        if (row.has_value(8))
        {
            this->Delia = ((Type63)(row.to_int32(8)));
        }
        this->SetKey(this->clement, this->gustily);
    }
    
    Table187Table::Table187Table()
    {
    }
    
    Table187Table::Table187Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table187Table::~Table187Table()
    {
    }
    
    void* Table187Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table187Row(row, ((Table187Table*)(table)));
    }
    
    const Table187Row* Table187Table::Find(Type_Gretta clement, time_t gustily) const
    {
        return this->FindRow(clement, gustily);
    }
    
    Table_AugRow::Table_AugRow(CremaReader::irow& row, Table_AugTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Presbyterian = row.to_datetime(0);
        this->SetKey(this->Presbyterian);
    }
    
    Table_AugTable::Table_AugTable()
    {
    }
    
    Table_AugTable::Table_AugTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_AugTable::~Table_AugTable()
    {
    }
    
    void* Table_AugTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_AugRow(row, ((Table_AugTable*)(table)));
    }
    
    const Table_AugRow* Table_AugTable::Find(time_t Presbyterian) const
    {
        return this->FindRow(Presbyterian);
    }
    
    Table181Row::Table181Row(CremaReader::irow& row, Table181Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Bostitch = ((Type40)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Bonnee = row.to_duration(1);
        }
        this->Mose = ((Type_HeraclitusDeletable)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->valuable = row.to_double(3);
        }
        this->Diahann = row.to_datetime(4);
        if (row.has_value(5))
        {
            this->pizzeria = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->bipartisanship = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->seagoing = row.to_string(7);
        }
        this->SetKey(this->Bostitch, this->Mose, this->Diahann);
    }
    
    Table181Table::Table181Table()
    {
    }
    
    Table181Table::Table181Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table181Table::~Table181Table()
    {
    }
    
    void* Table181Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table181Row(row, ((Table181Table*)(table)));
    }
    
    const Table181Row* Table181Table::Find(Type40 Bostitch, Type_HeraclitusDeletable Mose, time_t Diahann) const
    {
        return this->FindRow(Bostitch, Mose, Diahann);
    }
    
    Table49Child1Row::Table49Child1Row(CremaReader::irow& row, Table49Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->uninteresting = row.to_uint8(0);
        if (row.has_value(1))
        {
            this->Lafitte = row.to_double(1);
        }
        this->feater = row.to_double(2);
        if (row.has_value(3))
        {
            this->doggone = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->midi = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->filament = ((Type27)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Zorana = ((Type_consortia)(row.to_int32(6)));
        }
        this->highish = ((Type_Meiji)(row.to_int32(7)));
        this->SetKey(this->uninteresting, this->feater, this->highish);
    }
    
    Table49Child1Table::Table49Child1Table()
    {
    }
    
    Table49Child1Table::Table49Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table49Child1Table::Table49Child1Table(std::vector<Table49Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table49Child1Table::~Table49Child1Table()
    {
    }
    
    void* Table49Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table49Child1Row(row, ((Table49Child1Table*)(table)));
    }
    
    const Table49Child1Row* Table49Child1Table::Find(unsigned char uninteresting, double feater, Type_Meiji highish) const
    {
        return this->FindRow(uninteresting, feater, highish);
    }
    
    Table49Child1Table Table49Row::Child1Empty;
    
    Table49Row::Table49Row(CremaReader::irow& row, Table49Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->adviser = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->Peterson = ((Type_pledge)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->cumuli = ((Type24)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->analyzed = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->scowler = row.to_int64(4);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table49Row::Child1Empty);
        }
        this->SetKey(this->adviser);
    }
    
    void Table49SetChild1(Table49Row* target, const std::vector<Table49Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table49Child1Table(childs);
    }
    
    Table49Table::Table49Table()
    {
    }
    
    Table49Table::Table49Table(CremaReader::itable& table)
    {
        this->Child1 = new Table49Child1Table(table.dataset().tables()["Table49.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table49SetChild1);
    }
    
    Table49Table::~Table49Table()
    {
        delete this->Child1;
    }
    
    void* Table49Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table49Row(row, ((Table49Table*)(table)));
    }
    
    const Table49Row* Table49Table::Find(unsigned short adviser) const
    {
        return this->FindRow(adviser);
    }
    
    Table_cognizancesRow::Table_cognizancesRow(CremaReader::irow& row, Table_cognizancesTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Elicia = row.to_int8(0);
        if (row.has_value(1))
        {
            this->execrably = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->Knudsen = ((Type_rennet)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->bulky = ((Type15)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->fake = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->scintillation = row.to_single(5);
        }
        this->SetKey(this->Elicia);
    }
    
    Table_cognizancesTable::Table_cognizancesTable()
    {
    }
    
    Table_cognizancesTable::Table_cognizancesTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_cognizancesTable::~Table_cognizancesTable()
    {
    }
    
    void* Table_cognizancesTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_cognizancesRow(row, ((Table_cognizancesTable*)(table)));
    }
    
    const Table_cognizancesRow* Table_cognizancesTable::Find(char Elicia) const
    {
        return this->FindRow(Elicia);
    }
    
    Table145Row::Table145Row(CremaReader::irow& row, Table145Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->calligraphy = ((Type_farinaceous)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->obtruder = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->Hollerith = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->hellbent = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->hemispherical = row.to_uint8(4);
        }
        this->deathblow = row.to_double(5);
        this->SetKey(this->calligraphy, this->deathblow);
    }
    
    Table145Table::Table145Table()
    {
    }
    
    Table145Table::Table145Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table145Table::~Table145Table()
    {
    }
    
    void* Table145Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table145Row(row, ((Table145Table*)(table)));
    }
    
    const Table145Row* Table145Table::Find(Type_farinaceous calligraphy, double deathblow) const
    {
        return this->FindRow(calligraphy, deathblow);
    }
    
    Table26Row::Table26Row(CremaReader::irow& row, Table26Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->obsession = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->lingua = row.to_uint8(1);
        }
        if (row.has_value(2))
        {
            this->Hogarth = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->litheness = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->corruptive = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->lanolin = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->Macgregor = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->airplane = ((Type_supportedDeletable)(row.to_int32(7)));
        }
        if (row.has_value(8))
        {
            this->outstripped = row.to_single(8);
        }
        this->SetKey(this->obsession);
    }
    
    Table26Table::Table26Table()
    {
    }
    
    Table26Table::Table26Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table26Table::~Table26Table()
    {
    }
    
    void* Table26Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table26Row(row, ((Table26Table*)(table)));
    }
    
    const Table26Row* Table26Table::Find(unsigned long long obsession) const
    {
        return this->FindRow(obsession);
    }
    
    Table_adenoidChild1Row::Table_adenoidChild1Row(CremaReader::irow& row, Table_adenoidChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->pennis = ((Type50)(row.to_int32(0)));
        this->SetKey(this->pennis);
    }
    
    Table_adenoidChild1Table::Table_adenoidChild1Table()
    {
    }
    
    Table_adenoidChild1Table::Table_adenoidChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_adenoidChild1Table::Table_adenoidChild1Table(std::vector<Table_adenoidChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_adenoidChild1Table::~Table_adenoidChild1Table()
    {
    }
    
    void* Table_adenoidChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_adenoidChild1Row(row, ((Table_adenoidChild1Table*)(table)));
    }
    
    const Table_adenoidChild1Row* Table_adenoidChild1Table::Find(Type50 pennis) const
    {
        return this->FindRow(pennis);
    }
    
    Table_adenoidChild1Table Table_adenoidRow::Child1Empty;
    
    Table_adenoidRow::Table_adenoidRow(CremaReader::irow& row, Table_adenoidTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->misstep = ((Type8)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->Alistair = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->birth = ((Type15)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->Abdel = row.to_int16(3);
        }
        if (row.has_value(4))
        {
            this->cousinly = row.to_uint64(4);
        }
        if (row.has_value(5))
        {
            this->Paley = row.to_int16(5);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_adenoidRow::Child1Empty);
        }
        this->SetKey(this->misstep);
    }
    
    void Table_adenoidSetChild1(Table_adenoidRow* target, const std::vector<Table_adenoidChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_adenoidChild1Table(childs);
    }
    
    Table_adenoidTable::Table_adenoidTable()
    {
    }
    
    Table_adenoidTable::Table_adenoidTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_adenoidChild1Table(table.dataset().tables()["Table_adenoid.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_adenoidSetChild1);
    }
    
    Table_adenoidTable::~Table_adenoidTable()
    {
        delete this->Child1;
    }
    
    void* Table_adenoidTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_adenoidRow(row, ((Table_adenoidTable*)(table)));
    }
    
    const Table_adenoidRow* Table_adenoidTable::Find(Type8 misstep) const
    {
        return this->FindRow(misstep);
    }
    
    Table_visualizationChild1Row::Table_visualizationChild1Row(CremaReader::irow& row, Table_visualizationChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->parabolic = row.to_int32(0);
        if (row.has_value(1))
        {
            this->syncopation = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->steamroller = row.to_boolean(2);
        }
        this->rattrap = row.to_int16(3);
        if (row.has_value(4))
        {
            this->Redondo = row.to_uint32(4);
        }
        if (row.has_value(5))
        {
            this->peach = row.to_int8(5);
        }
        this->SetKey(this->parabolic, this->rattrap);
    }
    
    Table_visualizationChild1Table::Table_visualizationChild1Table()
    {
    }
    
    Table_visualizationChild1Table::Table_visualizationChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_visualizationChild1Table::Table_visualizationChild1Table(std::vector<Table_visualizationChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_visualizationChild1Table::~Table_visualizationChild1Table()
    {
    }
    
    void* Table_visualizationChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_visualizationChild1Row(row, ((Table_visualizationChild1Table*)(table)));
    }
    
    const Table_visualizationChild1Row* Table_visualizationChild1Table::Find(int parabolic, short rattrap) const
    {
        return this->FindRow(parabolic, rattrap);
    }
    
    Table_visualizationChild1Table Table_visualizationRow::Child1Empty;
    
    Table_visualizationRow::Table_visualizationRow(CremaReader::irow& row, Table_visualizationTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->marginalia = row.to_uint64(0);
        this->chatted = row.to_int64(1);
        if (row.has_value(2))
        {
            this->dutiful = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->strangulate = row.to_uint64(3);
        }
        if (row.has_value(4))
        {
            this->jazz = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->Rozella = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->patrician = row.to_int8(6);
        }
        if (row.has_value(7))
        {
            this->brunet = row.to_uint16(7);
        }
        if (row.has_value(8))
        {
            this->sweeping = row.to_int16(8);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_visualizationRow::Child1Empty);
        }
        this->SetKey(this->marginalia, this->chatted);
    }
    
    void Table_visualizationSetChild1(Table_visualizationRow* target, const std::vector<Table_visualizationChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_visualizationChild1Table(childs);
    }
    
    Table_visualizationTable::Table_visualizationTable()
    {
    }
    
    Table_visualizationTable::Table_visualizationTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_visualizationChild1Table(table.dataset().tables()["Table_visualization.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_visualizationSetChild1);
    }
    
    Table_visualizationTable::~Table_visualizationTable()
    {
        delete this->Child1;
    }
    
    void* Table_visualizationTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_visualizationRow(row, ((Table_visualizationTable*)(table)));
    }
    
    const Table_visualizationRow* Table_visualizationTable::Find(unsigned long long marginalia, long long chatted) const
    {
        return this->FindRow(marginalia, chatted);
    }
    
    Table147Row::Table147Row(CremaReader::irow& row, Table147Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->cuttlefish = row.to_duration(0);
        if (row.has_value(1))
        {
            this->cruse = row.to_uint64(1);
        }
        this->yonder = row.to_uint64(2);
        if (row.has_value(3))
        {
            this->Michell = ((Type25)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->rabbet = ((Type_primitiveness)(row.to_int32(4)));
        }
        if (row.has_value(5))
        {
            this->Sabra = row.to_duration(5);
        }
        if (row.has_value(6))
        {
            this->vulcanization = row.to_datetime(6);
        }
        if (row.has_value(7))
        {
            this->Ilka = row.to_uint16(7);
        }
        if (row.has_value(8))
        {
            this->Waldheim = row.to_boolean(8);
        }
        if (row.has_value(9))
        {
            this->Curtice = row.to_duration(9);
        }
        if (row.has_value(10))
        {
            this->anticlerical = row.to_int16(10);
        }
        if (row.has_value(11))
        {
            this->Pam = row.to_single(11);
        }
        this->SetKey(this->cuttlefish, this->yonder);
    }
    
    Table147Table::Table147Table()
    {
    }
    
    Table147Table::Table147Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table147Table::~Table147Table()
    {
    }
    
    void* Table147Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table147Row(row, ((Table147Table*)(table)));
    }
    
    const Table147Row* Table147Table::Find(int cuttlefish, unsigned long long yonder) const
    {
        return this->FindRow(cuttlefish, yonder);
    }
    
    Table197Row::Table197Row(CremaReader::irow& row, Table197Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->stablest = ((Type1)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->mighty = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->Austin = row.to_int32(2);
        }
        if (row.has_value(3))
        {
            this->bespeak = ((Type18)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->greenfield = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->crossbar = row.to_int16(5);
        }
        this->Waite = row.to_int32(6);
        this->SetKey(this->stablest, this->Waite);
    }
    
    Table197Table::Table197Table()
    {
    }
    
    Table197Table::Table197Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table197Table::~Table197Table()
    {
    }
    
    void* Table197Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table197Row(row, ((Table197Table*)(table)));
    }
    
    const Table197Row* Table197Table::Find(Type1 stablest, int Waite) const
    {
        return this->FindRow(stablest, Waite);
    }
    
    Table_blusteringRow::Table_blusteringRow(CremaReader::irow& row, Table_blusteringTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Tamma = ((Type25)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->thereby = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->jadeite = row.to_int32(2);
        }
        if (row.has_value(3))
        {
            this->rejoinder = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->meaningful = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->pilgrimage = row.to_string(5);
        }
        if (row.has_value(6))
        {
            this->monseigneur = ((Type25)(row.to_int32(6)));
        }
        this->SetKey(this->Tamma);
    }
    
    Table_blusteringTable::Table_blusteringTable()
    {
    }
    
    Table_blusteringTable::Table_blusteringTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_blusteringTable::~Table_blusteringTable()
    {
    }
    
    void* Table_blusteringTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_blusteringRow(row, ((Table_blusteringTable*)(table)));
    }
    
    const Table_blusteringRow* Table_blusteringTable::Find(Type25 Tamma) const
    {
        return this->FindRow(Tamma);
    }
    
    Table_creepyChild2Row::Table_creepyChild2Row(CremaReader::irow& row, Table_creepyChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->rift = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->detriment = row.to_boolean(1);
        }
        this->SetKey(this->rift);
    }
    
    Table_creepyChild2Table::Table_creepyChild2Table()
    {
    }
    
    Table_creepyChild2Table::Table_creepyChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_creepyChild2Table::Table_creepyChild2Table(std::vector<Table_creepyChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_creepyChild2Table::~Table_creepyChild2Table()
    {
    }
    
    void* Table_creepyChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_creepyChild2Row(row, ((Table_creepyChild2Table*)(table)));
    }
    
    const Table_creepyChild2Row* Table_creepyChild2Table::Find(unsigned short rift) const
    {
        return this->FindRow(rift);
    }
    
    Table_creepyChild2Table Table_creepyRow::Child2Empty;
    
    Table_creepyRow::Table_creepyRow(CremaReader::irow& row, Table_creepyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->flabbily = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->wasteful = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->litigiousness = row.to_uint64(2);
        }
        if (row.has_value(3))
        {
            this->chasm = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->Malena = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->admirable = ((Type_canted)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Thatcher = row.to_boolean(6);
        }
        if (row.has_value(7))
        {
            this->prowler = row.to_int32(7);
        }
        if (row.has_value(8))
        {
            this->Myrwyn = row.to_uint64(8);
        }
        if (row.has_value(9))
        {
            this->overbuilt = row.to_int32(9);
        }
        if (row.has_value(10))
        {
            this->bough = ((Type_insolent)(row.to_int32(10)));
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_creepyRow::Child2Empty);
        }
        this->SetKey(this->flabbily);
    }
    
    void Table_creepySetChild2(Table_creepyRow* target, const std::vector<Table_creepyChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_creepyChild2Table(childs);
    }
    
    Table_creepyTable::Table_creepyTable()
    {
    }
    
    Table_creepyTable::Table_creepyTable(CremaReader::itable& table)
    {
        this->Child2 = new Table_creepyChild2Table(table.dataset().tables()["Table_creepy.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child2->Rows, Table_creepySetChild2);
    }
    
    Table_creepyTable::~Table_creepyTable()
    {
        delete this->Child2;
    }
    
    void* Table_creepyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_creepyRow(row, ((Table_creepyTable*)(table)));
    }
    
    const Table_creepyRow* Table_creepyTable::Find(time_t flabbily) const
    {
        return this->FindRow(flabbily);
    }
    
    Table_teashopRow::Table_teashopRow(CremaReader::irow& row, Table_teashopTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->corpsman = row.to_double(0);
        if (row.has_value(1))
        {
            this->Araucanian = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Kaposi = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->hyperemia = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->pensiveness = row.to_single(4);
        }
        this->jetting = row.to_boolean(5);
        if (row.has_value(6))
        {
            this->babe = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->clears = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->codetermine = row.to_duration(8);
        }
        this->SetKey(this->corpsman, this->jetting);
    }
    
    Table_teashopTable::Table_teashopTable()
    {
    }
    
    Table_teashopTable::Table_teashopTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_teashopTable::~Table_teashopTable()
    {
    }
    
    void* Table_teashopTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_teashopRow(row, ((Table_teashopTable*)(table)));
    }
    
    const Table_teashopRow* Table_teashopTable::Find(double corpsman, bool jetting) const
    {
        return this->FindRow(corpsman, jetting);
    }
    
    Table178Row::Table178Row(CremaReader::irow& row, Table178Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->turk = row.to_single(0);
        if (row.has_value(1))
        {
            this->blankness = ((Type74)(row.to_int32(1)));
        }
        this->SetKey(this->turk);
    }
    
    Table178Table::Table178Table()
    {
    }
    
    Table178Table::Table178Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table178Table::~Table178Table()
    {
    }
    
    void* Table178Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table178Row(row, ((Table178Table*)(table)));
    }
    
    const Table178Row* Table178Table::Find(float turk) const
    {
        return this->FindRow(turk);
    }
    
    Table35Child1Row::Table35Child1Row(CremaReader::irow& row, Table35Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->forcefulness = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->Inglis = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->seismography = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->faultlessness = row.to_int32(3);
        }
        if (row.has_value(4))
        {
            this->fiver = row.to_uint16(4);
        }
        this->prevention = row.to_int16(5);
        if (row.has_value(6))
        {
            this->purloiner = row.to_datetime(6);
        }
        if (row.has_value(7))
        {
            this->Ancell = row.to_uint64(7);
        }
        this->SetKey(this->forcefulness, this->prevention);
    }
    
    Table35Child1Table::Table35Child1Table()
    {
    }
    
    Table35Child1Table::Table35Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table35Child1Table::Table35Child1Table(std::vector<Table35Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table35Child1Table::~Table35Child1Table()
    {
    }
    
    void* Table35Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table35Child1Row(row, ((Table35Child1Table*)(table)));
    }
    
    const Table35Child1Row* Table35Child1Table::Find(time_t forcefulness, short prevention) const
    {
        return this->FindRow(forcefulness, prevention);
    }
    
    Table35Child1Table Table35Row::Child1Empty;
    
    Table35Row::Table35Row(CremaReader::irow& row, Table35Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Portsmouth = row.to_int16(0);
        if (row.has_value(1))
        {
            this->bout = ((Type_rennet)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->aerosol = ((Type_HeraclitusDeletable)(row.to_int32(2)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table35Row::Child1Empty);
        }
        this->SetKey(this->Portsmouth);
    }
    
    void Table35SetChild1(Table35Row* target, const std::vector<Table35Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table35Child1Table(childs);
    }
    
    Table35Table::Table35Table()
    {
    }
    
    Table35Table::Table35Table(CremaReader::itable& table)
    {
        this->Child1 = new Table35Child1Table(table.dataset().tables()["Table35.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table35SetChild1);
    }
    
    Table35Table::~Table35Table()
    {
        delete this->Child1;
    }
    
    void* Table35Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table35Row(row, ((Table35Table*)(table)));
    }
    
    const Table35Row* Table35Table::Find(short Portsmouth) const
    {
        return this->FindRow(Portsmouth);
    }
    
    Table41Row::Table41Row(CremaReader::irow& row, Table41Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->epoxy = row.to_uint8(0);
        this->SetKey(this->epoxy);
    }
    
    Table41Table::Table41Table()
    {
    }
    
    Table41Table::Table41Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table41Table::~Table41Table()
    {
    }
    
    void* Table41Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table41Row(row, ((Table41Table*)(table)));
    }
    
    const Table41Row* Table41Table::Find(unsigned char epoxy) const
    {
        return this->FindRow(epoxy);
    }
    
    Table8Child1Row::Table8Child1Row(CremaReader::irow& row, Table8Child1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Englishmen = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->faze = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->degeneracy = row.to_uint64(2);
        }
        if (row.has_value(3))
        {
            this->studious = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->slouch = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->regularity = ((Type8)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->Bible = row.to_single(6);
        }
        if (row.has_value(7))
        {
            this->paunch = row.to_single(7);
        }
        this->SetKey(this->Englishmen);
    }
    
    Table8Child1Table::Table8Child1Table()
    {
    }
    
    Table8Child1Table::Table8Child1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table8Child1Table::Table8Child1Table(std::vector<Table8Child1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table8Child1Table::~Table8Child1Table()
    {
    }
    
    void* Table8Child1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table8Child1Row(row, ((Table8Child1Table*)(table)));
    }
    
    const Table8Child1Row* Table8Child1Table::Find(time_t Englishmen) const
    {
        return this->FindRow(Englishmen);
    }
    
    Table8Child_BCRow::Table8Child_BCRow(CremaReader::irow& row, Table8Child_BCTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Rodriquez = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->friendless = row.to_single(1);
        }
        this->SetKey(this->Rodriquez);
    }
    
    Table8Child_BCTable::Table8Child_BCTable()
    {
    }
    
    Table8Child_BCTable::Table8Child_BCTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table8Child_BCTable::Table8Child_BCTable(std::vector<Table8Child_BCRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table8Child_BCTable::~Table8Child_BCTable()
    {
    }
    
    void* Table8Child_BCTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table8Child_BCRow(row, ((Table8Child_BCTable*)(table)));
    }
    
    const Table8Child_BCRow* Table8Child_BCTable::Find(time_t Rodriquez) const
    {
        return this->FindRow(Rodriquez);
    }
    
    Table8Child1Table Table8Row::Child1Empty;
    
    Table8Child_BCTable Table8Row::Child_BCEmpty;
    
    Table8Row::Table8Row(CremaReader::irow& row, Table8Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Jeddy = row.to_single(0);
        if (row.has_value(1))
        {
            this->cramper = row.to_uint32(1);
        }
        if (row.has_value(2))
        {
            this->accumulative = row.to_uint32(2);
        }
        this->dimmest = row.to_boolean(3);
        if (row.has_value(4))
        {
            this->parity = row.to_double(4);
        }
        if (row.has_value(5))
        {
            this->SAC = row.to_boolean(5);
        }
        if (row.has_value(6))
        {
            this->plantlike = row.to_int32(6);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table8Row::Child1Empty);
        }
        if ((this->Child_BC == nullptr))
        {
            this->Child_BC = &(Table8Row::Child_BCEmpty);
        }
        this->SetKey(this->Jeddy, this->dimmest);
    }
    
    void Table8SetChild1(Table8Row* target, const std::vector<Table8Child1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table8Child1Table(childs);
    }
    
    void Table8SetChild_BC(Table8Row* target, const std::vector<Table8Child_BCRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_BC = new Table8Child_BCTable(childs);
    }
    
    Table8Table::Table8Table()
    {
    }
    
    Table8Table::Table8Table(CremaReader::itable& table)
    {
        this->Child1 = new Table8Child1Table(table.dataset().tables()["Table8.Child1"]);
        this->Child_BC = new Table8Child_BCTable(table.dataset().tables()["Table8.Child_BC"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table8SetChild1);
        this->SetRelations(this->Child_BC->Rows, Table8SetChild_BC);
    }
    
    Table8Table::~Table8Table()
    {
        delete this->Child1;
        delete this->Child_BC;
    }
    
    void* Table8Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table8Row(row, ((Table8Table*)(table)));
    }
    
    const Table8Row* Table8Table::Find(float Jeddy, bool dimmest) const
    {
        return this->FindRow(Jeddy, dimmest);
    }
    
    Table_annoyChild_ArrheniusRow::Table_annoyChild_ArrheniusRow(CremaReader::irow& row, Table_annoyChild_ArrheniusTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->gaping = row.to_int16(0);
        if (row.has_value(1))
        {
            this->optics = row.to_boolean(1);
        }
        if (row.has_value(2))
        {
            this->cowpony = row.to_double(2);
        }
        if (row.has_value(3))
        {
            this->Vannie = ((Type_Arlan)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->dibble = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->Orono = ((Type_Attn)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->moonstone = row.to_double(6);
        }
        this->illusion = row.to_duration(7);
        this->SetKey(this->gaping, this->illusion);
    }
    
    Table_annoyChild_ArrheniusTable::Table_annoyChild_ArrheniusTable()
    {
    }
    
    Table_annoyChild_ArrheniusTable::Table_annoyChild_ArrheniusTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_annoyChild_ArrheniusTable::Table_annoyChild_ArrheniusTable(std::vector<Table_annoyChild_ArrheniusRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_annoyChild_ArrheniusTable::~Table_annoyChild_ArrheniusTable()
    {
    }
    
    void* Table_annoyChild_ArrheniusTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_annoyChild_ArrheniusRow(row, ((Table_annoyChild_ArrheniusTable*)(table)));
    }
    
    const Table_annoyChild_ArrheniusRow* Table_annoyChild_ArrheniusTable::Find(short gaping, int illusion) const
    {
        return this->FindRow(gaping, illusion);
    }
    
    Table_annoyChild_ArrheniusTable Table_annoyRow::Child_ArrheniusEmpty;
    
    Table_annoyRow::Table_annoyRow(CremaReader::irow& row, Table_annoyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->corpsman = row.to_double(0);
        if (row.has_value(1))
        {
            this->Araucanian = row.to_uint16(1);
        }
        if (row.has_value(2))
        {
            this->Kaposi = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->hyperemia = row.to_single(3);
        }
        if (row.has_value(4))
        {
            this->pensiveness = row.to_single(4);
        }
        this->jetting = row.to_boolean(5);
        if (row.has_value(6))
        {
            this->babe = row.to_double(6);
        }
        if (row.has_value(7))
        {
            this->clears = row.to_single(7);
        }
        if (row.has_value(8))
        {
            this->codetermine = row.to_duration(8);
        }
        if (row.has_value(9))
        {
            this->lotion = row.to_int32(9);
        }
        if (row.has_value(10))
        {
            this->crosscurrent = row.to_int8(10);
        }
        if (row.has_value(11))
        {
            this->Ad = ((Type15)(row.to_int32(11)));
        }
        if (row.has_value(12))
        {
            this->neg = row.to_boolean(12);
        }
        if ((this->Child_Arrhenius == nullptr))
        {
            this->Child_Arrhenius = &(Table_annoyRow::Child_ArrheniusEmpty);
        }
        this->SetKey(this->corpsman, this->jetting);
    }
    
    void Table_annoySetChild_Arrhenius(Table_annoyRow* target, const std::vector<Table_annoyChild_ArrheniusRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Arrhenius = new Table_annoyChild_ArrheniusTable(childs);
    }
    
    Table_annoyTable::Table_annoyTable()
    {
    }
    
    Table_annoyTable::Table_annoyTable(CremaReader::itable& table)
    {
        this->Child_Arrhenius = new Table_annoyChild_ArrheniusTable(table.dataset().tables()["Table_annoy.Child_Arrhenius"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Arrhenius->Rows, Table_annoySetChild_Arrhenius);
    }
    
    Table_annoyTable::~Table_annoyTable()
    {
        delete this->Child_Arrhenius;
    }
    
    void* Table_annoyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_annoyRow(row, ((Table_annoyTable*)(table)));
    }
    
    const Table_annoyRow* Table_annoyTable::Find(double corpsman, bool jetting) const
    {
        return this->FindRow(corpsman, jetting);
    }
    
    Table_headdressRow::Table_headdressRow(CremaReader::irow& row, Table_headdressTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->culpableness = row.to_single(0);
        if (row.has_value(1))
        {
            this->spare = row.to_uint64(1);
        }
        if (row.has_value(2))
        {
            this->ltd = ((Type_Page)(row.to_int32(2)));
        }
        this->SetKey(this->culpableness);
    }
    
    Table_headdressTable::Table_headdressTable()
    {
    }
    
    Table_headdressTable::Table_headdressTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_headdressTable::~Table_headdressTable()
    {
    }
    
    void* Table_headdressTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_headdressRow(row, ((Table_headdressTable*)(table)));
    }
    
    const Table_headdressRow* Table_headdressTable::Find(float culpableness) const
    {
        return this->FindRow(culpableness);
    }
    
    Table_ValinaRow::Table_ValinaRow(CremaReader::irow& row, Table_ValinaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Tanny = row.to_double(0);
        if (row.has_value(1))
        {
            this->locale = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->fagoting = row.to_int8(2);
        }
        if (row.has_value(3))
        {
            this->aim = ((Type_Madison)(row.to_int32(3)));
        }
        this->brutality = row.to_string(4);
        if (row.has_value(5))
        {
            this->scantly = row.to_single(5);
        }
        if (row.has_value(6))
        {
            this->British = row.to_int32(6);
        }
        if (row.has_value(7))
        {
            this->broadcast = row.to_int8(7);
        }
        if (row.has_value(8))
        {
            this->injurer = row.to_single(8);
        }
        this->SetKey(this->Tanny, this->brutality);
    }
    
    Table_ValinaTable::Table_ValinaTable()
    {
    }
    
    Table_ValinaTable::Table_ValinaTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_ValinaTable::~Table_ValinaTable()
    {
    }
    
    void* Table_ValinaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_ValinaRow(row, ((Table_ValinaTable*)(table)));
    }
    
    const Table_ValinaRow* Table_ValinaTable::Find(double Tanny, const std::string& brutality) const
    {
        return this->FindRow(Tanny, brutality);
    }
    
    Table_WeinbergChild1Row::Table_WeinbergChild1Row(CremaReader::irow& row, Table_WeinbergChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->hunt = ((Type24)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->husband = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->homicide = ((Type_Multan)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->intrinsic = row.to_int8(3);
        }
        if (row.has_value(4))
        {
            this->iceberg = row.to_string(4);
        }
        this->SetKey(this->hunt);
    }
    
    Table_WeinbergChild1Table::Table_WeinbergChild1Table()
    {
    }
    
    Table_WeinbergChild1Table::Table_WeinbergChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_WeinbergChild1Table::Table_WeinbergChild1Table(std::vector<Table_WeinbergChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_WeinbergChild1Table::~Table_WeinbergChild1Table()
    {
    }
    
    void* Table_WeinbergChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_WeinbergChild1Row(row, ((Table_WeinbergChild1Table*)(table)));
    }
    
    const Table_WeinbergChild1Row* Table_WeinbergChild1Table::Find(Type24 hunt) const
    {
        return this->FindRow(hunt);
    }
    
    Table_WeinbergChild1Table Table_WeinbergRow::Child1Empty;
    
    Table_WeinbergRow::Table_WeinbergRow(CremaReader::irow& row, Table_WeinbergTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->unnavigable = row.to_single(0);
        if (row.has_value(1))
        {
            this->Janessa = row.to_uint64(1);
        }
        this->assiduity = row.to_datetime(2);
        if (row.has_value(3))
        {
            this->yardmaster = ((Type8)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->Sir = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->thermostat = ((Type_Attn)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->popinjay = row.to_string(6);
        }
        if (row.has_value(7))
        {
            this->planetarium = row.to_uint16(7);
        }
        if (row.has_value(8))
        {
            this->jasper = ((Type_insolent)(row.to_int32(8)));
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_WeinbergRow::Child1Empty);
        }
        this->SetKey(this->unnavigable, this->assiduity);
    }
    
    void Table_WeinbergSetChild1(Table_WeinbergRow* target, const std::vector<Table_WeinbergChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_WeinbergChild1Table(childs);
    }
    
    Table_WeinbergTable::Table_WeinbergTable()
    {
    }
    
    Table_WeinbergTable::Table_WeinbergTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_WeinbergChild1Table(table.dataset().tables()["Table_Weinberg.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_WeinbergSetChild1);
    }
    
    Table_WeinbergTable::~Table_WeinbergTable()
    {
        delete this->Child1;
    }
    
    void* Table_WeinbergTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_WeinbergRow(row, ((Table_WeinbergTable*)(table)));
    }
    
    const Table_WeinbergRow* Table_WeinbergTable::Find(float unnavigable, time_t assiduity) const
    {
        return this->FindRow(unnavigable, assiduity);
    }
    
    Table198Row::Table198Row(CremaReader::irow& row, Table198Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->nonscheduled = row.to_uint16(0);
        if (row.has_value(1))
        {
            this->mandrill = row.to_int64(1);
        }
        if (row.has_value(2))
        {
            this->Caro = ((Type85)(row.to_int32(2)));
        }
        this->SetKey(this->nonscheduled);
    }
    
    Table198Table::Table198Table()
    {
    }
    
    Table198Table::Table198Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table198Table::~Table198Table()
    {
    }
    
    void* Table198Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table198Row(row, ((Table198Table*)(table)));
    }
    
    const Table198Row* Table198Table::Find(unsigned short nonscheduled) const
    {
        return this->FindRow(nonscheduled);
    }
    
    Table_longsightedRow::Table_longsightedRow(CremaReader::irow& row, Table_longsightedTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->event = ((Type_Gretta)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->serendipitous = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->Pawtucket = row.to_duration(2);
        }
        this->cordial = row.to_string(3);
        this->SetKey(this->event, this->cordial);
    }
    
    Table_longsightedTable::Table_longsightedTable()
    {
    }
    
    Table_longsightedTable::Table_longsightedTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_longsightedTable::~Table_longsightedTable()
    {
    }
    
    void* Table_longsightedTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_longsightedRow(row, ((Table_longsightedTable*)(table)));
    }
    
    const Table_longsightedRow* Table_longsightedTable::Find(Type_Gretta event, const std::string& cordial) const
    {
        return this->FindRow(event, cordial);
    }
    
    Table161Row::Table161Row(CremaReader::irow& row, Table161Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->choral = ((Type70)(row.to_int32(0)));
        if (row.has_value(1))
        {
            this->malting = row.to_string(1);
        }
        this->intuitive = row.to_uint8(2);
        if (row.has_value(3))
        {
            this->bandsmen = row.to_uint16(3);
        }
        this->SetKey(this->choral, this->intuitive);
    }
    
    Table161Table::Table161Table()
    {
    }
    
    Table161Table::Table161Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table161Table::~Table161Table()
    {
    }
    
    void* Table161Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table161Row(row, ((Table161Table*)(table)));
    }
    
    const Table161Row* Table161Table::Find(Type70 choral, unsigned char intuitive) const
    {
        return this->FindRow(choral, intuitive);
    }
    
    Table_intervalChild1Row::Table_intervalChild1Row(CremaReader::irow& row, Table_intervalChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->cesspool = row.to_uint64(0);
        if (row.has_value(1))
        {
            this->vocalization = row.to_int8(1);
        }
        if (row.has_value(2))
        {
            this->overbook = row.to_boolean(2);
        }
        if (row.has_value(3))
        {
            this->affidavit = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->More = row.to_boolean(4);
        }
        this->philosophizer = row.to_duration(5);
        this->goatherd = row.to_int32(6);
        if (row.has_value(7))
        {
            this->bummed = ((Type_livingness)(row.to_int32(7)));
        }
        this->Shawnee = row.to_uint64(8);
        if (row.has_value(9))
        {
            this->wannabe = row.to_uint8(9);
        }
        if (row.has_value(10))
        {
            this->Tartuffe = row.to_datetime(10);
        }
        if (row.has_value(11))
        {
            this->Ernaline = row.to_single(11);
        }
        if (row.has_value(12))
        {
            this->idealistically = ((Type30)(row.to_int32(12)));
        }
        this->SetKey(this->cesspool, this->philosophizer, this->goatherd, this->Shawnee);
    }
    
    Table_intervalChild1Table::Table_intervalChild1Table()
    {
    }
    
    Table_intervalChild1Table::Table_intervalChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_intervalChild1Table::Table_intervalChild1Table(std::vector<Table_intervalChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_intervalChild1Table::~Table_intervalChild1Table()
    {
    }
    
    void* Table_intervalChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_intervalChild1Row(row, ((Table_intervalChild1Table*)(table)));
    }
    
    const Table_intervalChild1Row* Table_intervalChild1Table::Find(unsigned long long cesspool, int philosophizer, int goatherd, unsigned long long Shawnee) const
    {
        return this->FindRow(cesspool, philosophizer, goatherd, Shawnee);
    }
    
    Table_intervalChild2Row::Table_intervalChild2Row(CremaReader::irow& row, Table_intervalChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Carleton = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Smithson = ((Type_housebreaking)(row.to_int32(1)));
        }
        if (row.has_value(2))
        {
            this->lifeguard = row.to_boolean(2);
        }
        this->embellisher = row.to_single(3);
        if (row.has_value(4))
        {
            this->byroad = row.to_string(4);
        }
        if (row.has_value(5))
        {
            this->speedup = row.to_double(5);
        }
        this->SetKey(this->Carleton, this->embellisher);
    }
    
    Table_intervalChild2Table::Table_intervalChild2Table()
    {
    }
    
    Table_intervalChild2Table::Table_intervalChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_intervalChild2Table::Table_intervalChild2Table(std::vector<Table_intervalChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_intervalChild2Table::~Table_intervalChild2Table()
    {
    }
    
    void* Table_intervalChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_intervalChild2Row(row, ((Table_intervalChild2Table*)(table)));
    }
    
    const Table_intervalChild2Row* Table_intervalChild2Table::Find(int Carleton, float embellisher) const
    {
        return this->FindRow(Carleton, embellisher);
    }
    
    Table_intervalChild1Table Table_intervalRow::Child1Empty;
    
    Table_intervalChild2Table Table_intervalRow::Child2Empty;
    
    Table_intervalRow::Table_intervalRow(CremaReader::irow& row, Table_intervalTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Elicia = row.to_int8(0);
        if (row.has_value(1))
        {
            this->execrably = row.to_datetime(1);
        }
        if (row.has_value(2))
        {
            this->Knudsen = ((Type_rennet)(row.to_int32(2)));
        }
        if (row.has_value(3))
        {
            this->bulky = ((Type15)(row.to_int32(3)));
        }
        if (row.has_value(4))
        {
            this->fake = row.to_single(4);
        }
        if (row.has_value(5))
        {
            this->scintillation = row.to_single(5);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_intervalRow::Child1Empty);
        }
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_intervalRow::Child2Empty);
        }
        this->SetKey(this->Elicia);
    }
    
    void Table_intervalSetChild1(Table_intervalRow* target, const std::vector<Table_intervalChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_intervalChild1Table(childs);
    }
    
    void Table_intervalSetChild2(Table_intervalRow* target, const std::vector<Table_intervalChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_intervalChild2Table(childs);
    }
    
    Table_intervalTable::Table_intervalTable()
    {
    }
    
    Table_intervalTable::Table_intervalTable(CremaReader::itable& table)
    {
        this->Child1 = new Table_intervalChild1Table(table.dataset().tables()["Table_interval.Child1"]);
        this->Child2 = new Table_intervalChild2Table(table.dataset().tables()["Table_interval.Child2"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child1->Rows, Table_intervalSetChild1);
        this->SetRelations(this->Child2->Rows, Table_intervalSetChild2);
    }
    
    Table_intervalTable::~Table_intervalTable()
    {
        delete this->Child1;
        delete this->Child2;
    }
    
    void* Table_intervalTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_intervalRow(row, ((Table_intervalTable*)(table)));
    }
    
    const Table_intervalRow* Table_intervalTable::Find(char Elicia) const
    {
        return this->FindRow(Elicia);
    }
    
    Table_recopyChild2Row::Table_recopyChild2Row(CremaReader::irow& row, Table_recopyChild2Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Arawakan = row.to_single(0);
        if (row.has_value(1))
        {
            this->candelabra = row.to_int64(1);
        }
        if (row.has_value(2))
        {
            this->polonium = row.to_single(2);
        }
        if (row.has_value(3))
        {
            this->perigee = row.to_int16(3);
        }
        this->SetKey(this->Arawakan);
    }
    
    Table_recopyChild2Table::Table_recopyChild2Table()
    {
    }
    
    Table_recopyChild2Table::Table_recopyChild2Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_recopyChild2Table::Table_recopyChild2Table(std::vector<Table_recopyChild2Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_recopyChild2Table::~Table_recopyChild2Table()
    {
    }
    
    void* Table_recopyChild2Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_recopyChild2Row(row, ((Table_recopyChild2Table*)(table)));
    }
    
    const Table_recopyChild2Row* Table_recopyChild2Table::Find(float Arawakan) const
    {
        return this->FindRow(Arawakan);
    }
    
    Table_recopyChild3Row::Table_recopyChild3Row(CremaReader::irow& row, Table_recopyChild3Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->bathetic = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Langley = row.to_duration(1);
        }
        if (row.has_value(2))
        {
            this->mambo = ((Type72)(row.to_int32(2)));
        }
        this->anger = row.to_int32(3);
        if (row.has_value(4))
        {
            this->op = row.to_int64(4);
        }
        this->SetKey(this->bathetic, this->anger);
    }
    
    Table_recopyChild3Table::Table_recopyChild3Table()
    {
    }
    
    Table_recopyChild3Table::Table_recopyChild3Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_recopyChild3Table::Table_recopyChild3Table(std::vector<Table_recopyChild3Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_recopyChild3Table::~Table_recopyChild3Table()
    {
    }
    
    void* Table_recopyChild3Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_recopyChild3Row(row, ((Table_recopyChild3Table*)(table)));
    }
    
    const Table_recopyChild3Row* Table_recopyChild3Table::Find(int bathetic, int anger) const
    {
        return this->FindRow(bathetic, anger);
    }
    
    Table_recopyChild2Table Table_recopyRow::Child2Empty;
    
    Table_recopyChild3Table Table_recopyRow::Child3Empty;
    
    Table_recopyRow::Table_recopyRow(CremaReader::irow& row, Table_recopyTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->flaxseed = row.to_uint8(0);
        if ((this->Child2 == nullptr))
        {
            this->Child2 = &(Table_recopyRow::Child2Empty);
        }
        if ((this->Child3 == nullptr))
        {
            this->Child3 = &(Table_recopyRow::Child3Empty);
        }
        this->SetKey(this->flaxseed);
    }
    
    void Table_recopySetChild2(Table_recopyRow* target, const std::vector<Table_recopyChild2Row*>& childs)
    {
        SetParent(target, childs);
        target->Child2 = new Table_recopyChild2Table(childs);
    }
    
    void Table_recopySetChild3(Table_recopyRow* target, const std::vector<Table_recopyChild3Row*>& childs)
    {
        SetParent(target, childs);
        target->Child3 = new Table_recopyChild3Table(childs);
    }
    
    Table_recopyTable::Table_recopyTable()
    {
    }
    
    Table_recopyTable::Table_recopyTable(CremaReader::itable& table)
    {
        this->Child2 = new Table_recopyChild2Table(table.dataset().tables()["Table_recopy.Child2"]);
        this->Child3 = new Table_recopyChild3Table(table.dataset().tables()["Table_recopy.Child3"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child2->Rows, Table_recopySetChild2);
        this->SetRelations(this->Child3->Rows, Table_recopySetChild3);
    }
    
    Table_recopyTable::~Table_recopyTable()
    {
        delete this->Child2;
        delete this->Child3;
    }
    
    void* Table_recopyTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_recopyRow(row, ((Table_recopyTable*)(table)));
    }
    
    const Table_recopyRow* Table_recopyTable::Find(unsigned char flaxseed) const
    {
        return this->FindRow(flaxseed);
    }
    
    Table_RivaChild_JeevesRow::Table_RivaChild_JeevesRow(CremaReader::irow& row, Table_RivaChild_JeevesTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->banshee = row.to_int64(0);
        if (row.has_value(1))
        {
            this->contently = row.to_single(1);
        }
        if (row.has_value(2))
        {
            this->stickleback = row.to_uint32(2);
        }
        if (row.has_value(3))
        {
            this->Wendie = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->Elmira = row.to_uint8(4);
        }
        if (row.has_value(5))
        {
            this->play = row.to_uint32(5);
        }
        if (row.has_value(6))
        {
            this->duce = row.to_uint32(6);
        }
        if (row.has_value(7))
        {
            this->shortstop = row.to_double(7);
        }
        this->SetKey(this->banshee);
    }
    
    Table_RivaChild_JeevesTable::Table_RivaChild_JeevesTable()
    {
    }
    
    Table_RivaChild_JeevesTable::Table_RivaChild_JeevesTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_RivaChild_JeevesTable::Table_RivaChild_JeevesTable(std::vector<Table_RivaChild_JeevesRow*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_RivaChild_JeevesTable::~Table_RivaChild_JeevesTable()
    {
    }
    
    void* Table_RivaChild_JeevesTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_RivaChild_JeevesRow(row, ((Table_RivaChild_JeevesTable*)(table)));
    }
    
    const Table_RivaChild_JeevesRow* Table_RivaChild_JeevesTable::Find(long long banshee) const
    {
        return this->FindRow(banshee);
    }
    
    Table_RivaChild1Row::Table_RivaChild1Row(CremaReader::irow& row, Table_RivaChild1Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->mule = row.to_string(0);
        if (row.has_value(1))
        {
            this->cycle = ((Type_canted)(row.to_int32(1)));
        }
        this->timpani = row.to_uint32(2);
        if (row.has_value(3))
        {
            this->merited = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->advisable = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->calculus = row.to_datetime(5);
        }
        if (row.has_value(6))
        {
            this->Groton = row.to_int64(6);
        }
        if (row.has_value(7))
        {
            this->scruple = ((Type_Meiji)(row.to_int32(7)));
        }
        this->SetKey(this->mule, this->timpani);
    }
    
    Table_RivaChild1Table::Table_RivaChild1Table()
    {
    }
    
    Table_RivaChild1Table::Table_RivaChild1Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_RivaChild1Table::Table_RivaChild1Table(std::vector<Table_RivaChild1Row*> rows)
    {
        this->ReadFromRows(rows);
    }
    
    Table_RivaChild1Table::~Table_RivaChild1Table()
    {
    }
    
    void* Table_RivaChild1Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_RivaChild1Row(row, ((Table_RivaChild1Table*)(table)));
    }
    
    const Table_RivaChild1Row* Table_RivaChild1Table::Find(const std::string& mule, unsigned int timpani) const
    {
        return this->FindRow(mule, timpani);
    }
    
    Table_RivaChild_JeevesTable Table_RivaRow::Child_JeevesEmpty;
    
    Table_RivaChild1Table Table_RivaRow::Child1Empty;
    
    Table_RivaRow::Table_RivaRow(CremaReader::irow& row, Table_RivaTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Tupperware = row.to_uint32(0);
        if (row.has_value(1))
        {
            this->outwitted = row.to_int32(1);
        }
        if (row.has_value(2))
        {
            this->Janeiro = row.to_uint8(2);
        }
        if (row.has_value(3))
        {
            this->capping = row.to_duration(3);
        }
        if (row.has_value(4))
        {
            this->whole = row.to_int8(4);
        }
        if (row.has_value(5))
        {
            this->convalescent = row.to_uint8(5);
        }
        if (row.has_value(6))
        {
            this->mayflower = row.to_int16(6);
        }
        if (row.has_value(7))
        {
            this->terrazzo = row.to_uint8(7);
        }
        if ((this->Child_Jeeves == nullptr))
        {
            this->Child_Jeeves = &(Table_RivaRow::Child_JeevesEmpty);
        }
        if ((this->Child1 == nullptr))
        {
            this->Child1 = &(Table_RivaRow::Child1Empty);
        }
        this->SetKey(this->Tupperware);
    }
    
    void Table_RivaSetChild_Jeeves(Table_RivaRow* target, const std::vector<Table_RivaChild_JeevesRow*>& childs)
    {
        SetParent(target, childs);
        target->Child_Jeeves = new Table_RivaChild_JeevesTable(childs);
    }
    
    void Table_RivaSetChild1(Table_RivaRow* target, const std::vector<Table_RivaChild1Row*>& childs)
    {
        SetParent(target, childs);
        target->Child1 = new Table_RivaChild1Table(childs);
    }
    
    Table_RivaTable::Table_RivaTable()
    {
    }
    
    Table_RivaTable::Table_RivaTable(CremaReader::itable& table)
    {
        this->Child_Jeeves = new Table_RivaChild_JeevesTable(table.dataset().tables()["Table_Riva.Child_Jeeves"]);
        this->Child1 = new Table_RivaChild1Table(table.dataset().tables()["Table_Riva.Child1"]);
        this->ReadFromTable(table);
        this->SetRelations(this->Child_Jeeves->Rows, Table_RivaSetChild_Jeeves);
        this->SetRelations(this->Child1->Rows, Table_RivaSetChild1);
    }
    
    Table_RivaTable::~Table_RivaTable()
    {
        delete this->Child_Jeeves;
        delete this->Child1;
    }
    
    void* Table_RivaTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_RivaRow(row, ((Table_RivaTable*)(table)));
    }
    
    const Table_RivaRow* Table_RivaTable::Find(unsigned int Tupperware) const
    {
        return this->FindRow(Tupperware);
    }
    
    Table_TamarraRow::Table_TamarraRow(CremaReader::irow& row, Table_TamarraTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Corbet = row.to_datetime(0);
        if (row.has_value(1))
        {
            this->LyX = row.to_int64(1);
        }
        this->gasser = ((Type_RhodesDeletable)(row.to_int32(2)));
        if (row.has_value(3))
        {
            this->salinity = row.to_datetime(3);
        }
        if (row.has_value(4))
        {
            this->Sanchez = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->Ephrayim = ((Type_lustilyDeletable)(row.to_int32(5)));
        }
        if (row.has_value(6))
        {
            this->tickle = ((Type_guttering)(row.to_int32(6)));
        }
        this->climate = ((Type_insolent)(row.to_int32(7)));
        this->SetKey(this->Corbet, this->gasser, this->climate);
    }
    
    Table_TamarraTable::Table_TamarraTable()
    {
    }
    
    Table_TamarraTable::Table_TamarraTable(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table_TamarraTable::~Table_TamarraTable()
    {
    }
    
    void* Table_TamarraTable::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table_TamarraRow(row, ((Table_TamarraTable*)(table)));
    }
    
    const Table_TamarraRow* Table_TamarraTable::Find(time_t Corbet, Type_RhodesDeletable gasser, Type_insolent climate) const
    {
        return this->FindRow(Corbet, gasser, climate);
    }
    
    Table190Row::Table190Row(CremaReader::irow& row, Table190Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->guessable = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Kevlar = row.to_double(1);
        }
        if (row.has_value(2))
        {
            this->tatterdemalion = row.to_uint16(2);
        }
        this->SetKey(this->guessable);
    }
    
    Table190Table::Table190Table()
    {
    }
    
    Table190Table::Table190Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table190Table::~Table190Table()
    {
    }
    
    void* Table190Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table190Row(row, ((Table190Table*)(table)));
    }
    
    const Table190Row* Table190Table::Find(int guessable) const
    {
        return this->FindRow(guessable);
    }
    
    Table156Row::Table156Row(CremaReader::irow& row, Table156Table* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Jolla = ((Type12)(row.to_int32(0)));
        this->thirster = row.to_int16(1);
        if (row.has_value(2))
        {
            this->claque = row.to_int32(2);
        }
        this->Klingon = row.to_int32(3);
        if (row.has_value(4))
        {
            this->ascendancy = row.to_datetime(4);
        }
        if (row.has_value(5))
        {
            this->Merci = ((Type_where)(row.to_int32(5)));
        }
        this->SetKey(this->Jolla, this->thirster, this->Klingon);
    }
    
    Table156Table::Table156Table()
    {
    }
    
    Table156Table::Table156Table(CremaReader::itable& table)
    {
        this->ReadFromTable(table);
    }
    
    Table156Table::~Table156Table()
    {
    }
    
    void* Table156Table::CreateRow(CremaReader::irow& row, void* table)
    {
        return new Table156Row(row, ((Table156Table*)(table)));
    }
    
    const Table156Row* Table156Table::Find(Type12 Jolla, short thirster, int Klingon) const
    {
        return this->FindRow(Jolla, thirster, Klingon);
    }
    
    CremaDataSet::CremaDataSet(CremaReader::idataset& dataSet)
    {
        this->Table_inerrant = new Table_inerrantTable(dataSet.tables()["Table_inerrant"]);
        this->Table204 = new Table204Table(dataSet.tables()["Table204"]);
        this->Table_Carol = new Table_CarolTable(dataSet.tables()["Table_Carol"]);
        this->Table141 = new Table141Table(dataSet.tables()["Table141"]);
        this->Table40 = new Table40Table(dataSet.tables()["Table40"]);
        this->Table57 = new Table57Table(dataSet.tables()["Table57"]);
        this->Table206 = new Table206Table(dataSet.tables()["Table206"]);
        this->Table_conical = new Table_conicalTable(dataSet.tables()["Table_conical"]);
        this->Table22 = new Table22Table(dataSet.tables()["Table22"]);
        this->Table_hydrosphere = new Table_hydrosphereTable(dataSet.tables()["Table_hydrosphere"]);
        this->Table46 = new Table46Table(dataSet.tables()["Table46"]);
        this->Table_agapae = new Table_agapaeTable(dataSet.tables()["Table_agapae"]);
        this->Table10 = new Table10Table(dataSet.tables()["Table10"]);
        this->Table_Carolan = new Table_CarolanTable(dataSet.tables()["Table_Carolan"]);
        this->Table_repressiveness = new Table_repressivenessTable(dataSet.tables()["Table_repressiveness"]);
        this->Table183 = new Table183Table(dataSet.tables()["Table183"]);
        this->Table94 = new Table94Table(dataSet.tables()["Table94"]);
        this->Table82 = new Table82Table(dataSet.tables()["Table82"]);
        this->Table87 = new Table87Table(dataSet.tables()["Table87"]);
        this->Table_implacableness = new Table_implacablenessTable(dataSet.tables()["Table_implacableness"]);
        this->Table_fleeing = new Table_fleeingTable(dataSet.tables()["Table_fleeing"]);
        this->Table50 = new Table50Table(dataSet.tables()["Table50"]);
        this->Table53 = new Table53Table(dataSet.tables()["Table53"]);
        this->Table89 = new Table89Table(dataSet.tables()["Table89"]);
        this->Table109 = new Table109Table(dataSet.tables()["Table109"]);
        this->Table180 = new Table180Table(dataSet.tables()["Table180"]);
        this->Table20 = new Table20Table(dataSet.tables()["Table20"]);
        this->Table21 = new Table21Table(dataSet.tables()["Table21"]);
        this->Table43 = new Table43Table(dataSet.tables()["Table43"]);
        this->Table51 = new Table51Table(dataSet.tables()["Table51"]);
        this->Table108 = new Table108Table(dataSet.tables()["Table108"]);
        this->Table76 = new Table76Table(dataSet.tables()["Table76"]);
        this->Table_duxes = new Table_duxesTable(dataSet.tables()["Table_duxes"]);
        this->Table196 = new Table196Table(dataSet.tables()["Table196"]);
        this->Table150 = new Table150Table(dataSet.tables()["Table150"]);
        this->Table_replenish = new Table_replenishTable(dataSet.tables()["Table_replenish"]);
        this->Table_bedpan = new Table_bedpanTable(dataSet.tables()["Table_bedpan"]);
        this->Table_wrist = new Table_wristTable(dataSet.tables()["Table_wrist"]);
        this->Table33 = new Table33Table(dataSet.tables()["Table33"]);
        this->Table_drawn = new Table_drawnTable(dataSet.tables()["Table_drawn"]);
        this->Table_Hallinan = new Table_HallinanTable(dataSet.tables()["Table_Hallinan"]);
        this->Table175 = new Table175Table(dataSet.tables()["Table175"]);
        this->Table85 = new Table85Table(dataSet.tables()["Table85"]);
        this->Table151 = new Table151Table(dataSet.tables()["Table151"]);
        this->Table_alien = new Table_alienTable(dataSet.tables()["Table_alien"]);
        this->Table_crystallizes = new Table_crystallizesTable(dataSet.tables()["Table_crystallizes"]);
        this->Table112 = new Table112Table(dataSet.tables()["Table112"]);
        this->Table116 = new Table116Table(dataSet.tables()["Table116"]);
        this->Table199 = new Table199Table(dataSet.tables()["Table199"]);
        this->Table9 = new Table9Table(dataSet.tables()["Table9"]);
        this->Table_piquantness = new Table_piquantnessTable(dataSet.tables()["Table_piquantness"]);
        this->Table111 = new Table111Table(dataSet.tables()["Table111"]);
        this->Table28 = new Table28Table(dataSet.tables()["Table28"]);
        this->Table_survey = new Table_surveyTable(dataSet.tables()["Table_survey"]);
        this->Table128 = new Table128Table(dataSet.tables()["Table128"]);
        this->Table31 = new Table31Table(dataSet.tables()["Table31"]);
        this->Table34 = new Table34Table(dataSet.tables()["Table34"]);
        this->Table_stockpile = new Table_stockpileTable(dataSet.tables()["Table_stockpile"]);
        this->Table44 = new Table44Table(dataSet.tables()["Table44"]);
        this->Table67 = new Table67Table(dataSet.tables()["Table67"]);
        this->Table_accident = new Table_accidentTable(dataSet.tables()["Table_accident"]);
        this->Table_symbiotic = new Table_symbioticTable(dataSet.tables()["Table_symbiotic"]);
        this->Table_Thornburg = new Table_ThornburgTable(dataSet.tables()["Table_Thornburg"]);
        this->Table_vixenish = new Table_vixenishTable(dataSet.tables()["Table_vixenish"]);
        this->Table142 = new Table142Table(dataSet.tables()["Table142"]);
        this->Table96 = new Table96Table(dataSet.tables()["Table96"]);
        this->Table98 = new Table98Table(dataSet.tables()["Table98"]);
        this->Table_refunder = new Table_refunderTable(dataSet.tables()["Table_refunder"]);
        this->Table_wolves = new Table_wolvesTable(dataSet.tables()["Table_wolves"]);
        this->Table45 = new Table45Table(dataSet.tables()["Table45"]);
        this->Table83 = new Table83Table(dataSet.tables()["Table83"]);
        this->Table_sherbet = new Table_sherbetTable(dataSet.tables()["Table_sherbet"]);
        this->Table_freighter = new Table_freighterTable(dataSet.tables()["Table_freighter"]);
        this->Table_Hanoverian = new Table_HanoverianTable(dataSet.tables()["Table_Hanoverian"]);
        this->Table_houri = new Table_houriTable(dataSet.tables()["Table_houri"]);
        this->Table191 = new Table191Table(dataSet.tables()["Table191"]);
        this->Table_lunchtime = new Table_lunchtimeTable(dataSet.tables()["Table_lunchtime"]);
        this->Table165 = new Table165Table(dataSet.tables()["Table165"]);
        this->Table_dreamless = new Table_dreamlessTable(dataSet.tables()["Table_dreamless"]);
        this->Table81 = new Table81Table(dataSet.tables()["Table81"]);
        this->Table_chicane = new Table_chicaneTable(dataSet.tables()["Table_chicane"]);
        this->Table90 = new Table90Table(dataSet.tables()["Table90"]);
        this->Table203 = new Table203Table(dataSet.tables()["Table203"]);
        this->Table_attachment = new Table_attachmentTable(dataSet.tables()["Table_attachment"]);
        this->Table195 = new Table195Table(dataSet.tables()["Table195"]);
        this->Table_Pamirs = new Table_PamirsTable(dataSet.tables()["Table_Pamirs"]);
        this->Table_kinder = new Table_kinderTable(dataSet.tables()["Table_kinder"]);
        this->Table_deathless = new Table_deathlessTable(dataSet.tables()["Table_deathless"]);
        this->Table117 = new Table117Table(dataSet.tables()["Table117"]);
        this->Table_selection = new Table_selectionTable(dataSet.tables()["Table_selection"]);
        this->Table1 = new Table1Table(dataSet.tables()["Table1"]);
        this->Table68 = new Table68Table(dataSet.tables()["Table68"]);
        this->Table_emotionless = new Table_emotionlessTable(dataSet.tables()["Table_emotionless"]);
        this->Table_halter = new Table_halterTable(dataSet.tables()["Table_halter"]);
        this->Table_studio = new Table_studioTable(dataSet.tables()["Table_studio"]);
        this->Table115 = new Table115Table(dataSet.tables()["Table115"]);
        this->Table182 = new Table182Table(dataSet.tables()["Table182"]);
        this->Table_sun = new Table_sunTable(dataSet.tables()["Table_sun"]);
        this->Table_license = new Table_licenseTable(dataSet.tables()["Table_license"]);
        this->Table129 = new Table129Table(dataSet.tables()["Table129"]);
        this->Table_navigable = new Table_navigableTable(dataSet.tables()["Table_navigable"]);
        this->Table_anchor = new Table_anchorTable(dataSet.tables()["Table_anchor"]);
        this->Table_Leontine = new Table_LeontineTable(dataSet.tables()["Table_Leontine"]);
        this->Table149 = new Table149Table(dataSet.tables()["Table149"]);
        this->Table17 = new Table17Table(dataSet.tables()["Table17"]);
        this->Table_hoodlum = new Table_hoodlumTable(dataSet.tables()["Table_hoodlum"]);
        this->Table138 = new Table138Table(dataSet.tables()["Table138"]);
        this->Table88 = new Table88Table(dataSet.tables()["Table88"]);
        this->Table_Bonnee = new Table_BonneeTable(dataSet.tables()["Table_Bonnee"]);
        this->Table_Joycelin = new Table_JoycelinTable(dataSet.tables()["Table_Joycelin"]);
        this->Table_reticulate = new Table_reticulateTable(dataSet.tables()["Table_reticulate"]);
        this->Table114 = new Table114Table(dataSet.tables()["Table114"]);
        this->Table118 = new Table118Table(dataSet.tables()["Table118"]);
        this->Table170 = new Table170Table(dataSet.tables()["Table170"]);
        this->Table64 = new Table64Table(dataSet.tables()["Table64"]);
        this->Table_designed = new Table_designedTable(dataSet.tables()["Table_designed"]);
        this->Table_scout = new Table_scoutTable(dataSet.tables()["Table_scout"]);
        this->Table_summarizer = new Table_summarizerTable(dataSet.tables()["Table_summarizer"]);
        this->Table132 = new Table132Table(dataSet.tables()["Table132"]);
        this->Table133 = new Table133Table(dataSet.tables()["Table133"]);
        this->Table154 = new Table154Table(dataSet.tables()["Table154"]);
        this->Table_fitted = new Table_fittedTable(dataSet.tables()["Table_fitted"]);
        this->Table_Tupungato = new Table_TupungatoTable(dataSet.tables()["Table_Tupungato"]);
        this->Table130 = new Table130Table(dataSet.tables()["Table130"]);
        this->Table200 = new Table200Table(dataSet.tables()["Table200"]);
        this->Table_capsulize = new Table_capsulizeTable(dataSet.tables()["Table_capsulize"]);
        this->Table210 = new Table210Table(dataSet.tables()["Table210"]);
        this->Table192 = new Table192Table(dataSet.tables()["Table192"]);
        this->Table_adulthood = new Table_adulthoodTable(dataSet.tables()["Table_adulthood"]);
        this->Table_gerrymander = new Table_gerrymanderTable(dataSet.tables()["Table_gerrymander"]);
        this->Table193 = new Table193Table(dataSet.tables()["Table193"]);
        this->Table_needlessness = new Table_needlessnessTable(dataSet.tables()["Table_needlessness"]);
        this->Table24 = new Table24Table(dataSet.tables()["Table24"]);
        this->Table62 = new Table62Table(dataSet.tables()["Table62"]);
        this->Table_expansionary = new Table_expansionaryTable(dataSet.tables()["Table_expansionary"]);
        this->Table_Giacinta = new Table_GiacintaTable(dataSet.tables()["Table_Giacinta"]);
        this->Table_Ianthe = new Table_IantheTable(dataSet.tables()["Table_Ianthe"]);
        this->Table_bootprints = new Table_bootprintsTable(dataSet.tables()["Table_bootprints"]);
        this->Table102 = new Table102Table(dataSet.tables()["Table102"]);
        this->Table164 = new Table164Table(dataSet.tables()["Table164"]);
        this->Table120 = new Table120Table(dataSet.tables()["Table120"]);
        this->Table158 = new Table158Table(dataSet.tables()["Table158"]);
        this->Table_gynecologic = new Table_gynecologicTable(dataSet.tables()["Table_gynecologic"]);
        this->Table169 = new Table169Table(dataSet.tables()["Table169"]);
        this->Table6 = new Table6Table(dataSet.tables()["Table6"]);
        this->Table172 = new Table172Table(dataSet.tables()["Table172"]);
        this->Table_computing = new Table_computingTable(dataSet.tables()["Table_computing"]);
        this->Table124 = new Table124Table(dataSet.tables()["Table124"]);
        this->Table106 = new Table106Table(dataSet.tables()["Table106"]);
        this->Table23 = new Table23Table(dataSet.tables()["Table23"]);
        this->Table60 = new Table60Table(dataSet.tables()["Table60"]);
        this->Table_Kit = new Table_KitTable(dataSet.tables()["Table_Kit"]);
        this->Table189 = new Table189Table(dataSet.tables()["Table189"]);
        this->Table105 = new Table105Table(dataSet.tables()["Table105"]);
        this->Table15 = new Table15Table(dataSet.tables()["Table15"]);
        this->Table16 = new Table16Table(dataSet.tables()["Table16"]);
        this->Table32 = new Table32Table(dataSet.tables()["Table32"]);
        this->Table4 = new Table4Table(dataSet.tables()["Table4"]);
        this->Table48 = new Table48Table(dataSet.tables()["Table48"]);
        this->Table_apocryphalness = new Table_apocryphalnessTable(dataSet.tables()["Table_apocryphalness"]);
        this->Table_intactness = new Table_intactnessTable(dataSet.tables()["Table_intactness"]);
        this->Table_Caribbean = new Table_CaribbeanTable(dataSet.tables()["Table_Caribbean"]);
        this->Table_crupper = new Table_crupperTable(dataSet.tables()["Table_crupper"]);
        this->Table101 = new Table101Table(dataSet.tables()["Table101"]);
        this->Table14 = new Table14Table(dataSet.tables()["Table14"]);
        this->Table2 = new Table2Table(dataSet.tables()["Table2"]);
        this->Table29 = new Table29Table(dataSet.tables()["Table29"]);
        this->Table36 = new Table36Table(dataSet.tables()["Table36"]);
        this->Table79 = new Table79Table(dataSet.tables()["Table79"]);
        this->Table_Briticism = new Table_BriticismTable(dataSet.tables()["Table_Briticism"]);
        this->Table107 = new Table107Table(dataSet.tables()["Table107"]);
        this->Table_Antony = new Table_AntonyTable(dataSet.tables()["Table_Antony"]);
        this->Table140 = new Table140Table(dataSet.tables()["Table140"]);
        this->Table27 = new Table27Table(dataSet.tables()["Table27"]);
        this->Table_Judd = new Table_JuddTable(dataSet.tables()["Table_Judd"]);
        this->Table18 = new Table18Table(dataSet.tables()["Table18"]);
        this->Table78 = new Table78Table(dataSet.tables()["Table78"]);
        this->Table_depart = new Table_departTable(dataSet.tables()["Table_depart"]);
        this->Table_Susanne = new Table_SusanneTable(dataSet.tables()["Table_Susanne"]);
        this->Table126 = new Table126Table(dataSet.tables()["Table126"]);
        this->Table146 = new Table146Table(dataSet.tables()["Table146"]);
        this->Table80 = new Table80Table(dataSet.tables()["Table80"]);
        this->Table_scleroses = new Table_sclerosesTable(dataSet.tables()["Table_scleroses"]);
        this->Table177 = new Table177Table(dataSet.tables()["Table177"]);
        this->Table75 = new Table75Table(dataSet.tables()["Table75"]);
        this->Table84 = new Table84Table(dataSet.tables()["Table84"]);
        this->Table_flange = new Table_flangeTable(dataSet.tables()["Table_flange"]);
        this->Table_Fonz = new Table_FonzTable(dataSet.tables()["Table_Fonz"]);
        this->Table_Melitta = new Table_MelittaTable(dataSet.tables()["Table_Melitta"]);
        this->Table_quartile = new Table_quartileTable(dataSet.tables()["Table_quartile"]);
        this->Table12 = new Table12Table(dataSet.tables()["Table12"]);
        this->Table202 = new Table202Table(dataSet.tables()["Table202"]);
        this->Table3 = new Table3Table(dataSet.tables()["Table3"]);
        this->Table63 = new Table63Table(dataSet.tables()["Table63"]);
        this->Table136 = new Table136Table(dataSet.tables()["Table136"]);
        this->Table66 = new Table66Table(dataSet.tables()["Table66"]);
        this->Table113 = new Table113Table(dataSet.tables()["Table113"]);
        this->Table52 = new Table52Table(dataSet.tables()["Table52"]);
        this->Table_glide = new Table_glideTable(dataSet.tables()["Table_glide"]);
        this->Table179 = new Table179Table(dataSet.tables()["Table179"]);
        this->Table_wingless = new Table_winglessTable(dataSet.tables()["Table_wingless"]);
        this->Table137 = new Table137Table(dataSet.tables()["Table137"]);
        this->Table_Ito = new Table_ItoTable(dataSet.tables()["Table_Ito"]);
        this->Table_Katheryn = new Table_KatherynTable(dataSet.tables()["Table_Katheryn"]);
        this->Table_Lucius = new Table_LuciusTable(dataSet.tables()["Table_Lucius"]);
        this->Table_Maxwell = new Table_MaxwellTable(dataSet.tables()["Table_Maxwell"]);
        this->Table_bang = new Table_bangTable(dataSet.tables()["Table_bang"]);
        this->Table_lineman = new Table_linemanTable(dataSet.tables()["Table_lineman"]);
        this->Table100 = new Table100Table(dataSet.tables()["Table100"]);
        this->Table74 = new Table74Table(dataSet.tables()["Table74"]);
        this->Table97 = new Table97Table(dataSet.tables()["Table97"]);
        this->Table_archaically = new Table_archaicallyTable(dataSet.tables()["Table_archaically"]);
        this->Table_codename = new Table_codenameTable(dataSet.tables()["Table_codename"]);
        this->Table_protectiveness = new Table_protectivenessTable(dataSet.tables()["Table_protectiveness"]);
        this->Table_dissuasive = new Table_dissuasiveTable(dataSet.tables()["Table_dissuasive"]);
        this->Table110 = new Table110Table(dataSet.tables()["Table110"]);
        this->Table_relent = new Table_relentTable(dataSet.tables()["Table_relent"]);
        this->Table_chromatography = new Table_chromatographyTable(dataSet.tables()["Table_chromatography"]);
        this->Table_CPI = new Table_CPITable(dataSet.tables()["Table_CPI"]);
        this->Table_metempsychoses = new Table_metempsychosesTable(dataSet.tables()["Table_metempsychoses"]);
        this->Table122 = new Table122Table(dataSet.tables()["Table122"]);
        this->Table134 = new Table134Table(dataSet.tables()["Table134"]);
        this->Table25 = new Table25Table(dataSet.tables()["Table25"]);
        this->Table73 = new Table73Table(dataSet.tables()["Table73"]);
        this->Table_oxidizes = new Table_oxidizesTable(dataSet.tables()["Table_oxidizes"]);
        this->Table_Kerouac = new Table_KerouacTable(dataSet.tables()["Table_Kerouac"]);
        this->Table13 = new Table13Table(dataSet.tables()["Table13"]);
        this->Table_ganglia = new Table_gangliaTable(dataSet.tables()["Table_ganglia"]);
        this->Table_approve = new Table_approveTable(dataSet.tables()["Table_approve"]);
        this->Table_SC = new Table_SCTable(dataSet.tables()["Table_SC"]);
        this->Table_Reade = new Table_ReadeTable(dataSet.tables()["Table_Reade"]);
        this->Table201 = new Table201Table(dataSet.tables()["Table201"]);
        this->Table_culprit = new Table_culpritTable(dataSet.tables()["Table_culprit"]);
        this->Table_tsunami = new Table_tsunamiTable(dataSet.tables()["Table_tsunami"]);
        this->Table135 = new Table135Table(dataSet.tables()["Table135"]);
        this->Table19 = new Table19Table(dataSet.tables()["Table19"]);
        this->Table37 = new Table37Table(dataSet.tables()["Table37"]);
        this->Table_alibi = new Table_alibiTable(dataSet.tables()["Table_alibi"]);
        this->Table38 = new Table38Table(dataSet.tables()["Table38"]);
        this->Table58 = new Table58Table(dataSet.tables()["Table58"]);
        this->Table_consanguineous = new Table_consanguineousTable(dataSet.tables()["Table_consanguineous"]);
        this->Table42 = new Table42Table(dataSet.tables()["Table42"]);
        this->Table72 = new Table72Table(dataSet.tables()["Table72"]);
        this->Table92 = new Table92Table(dataSet.tables()["Table92"]);
        this->Table99 = new Table99Table(dataSet.tables()["Table99"]);
        this->Table_Jerry = new Table_JerryTable(dataSet.tables()["Table_Jerry"]);
        this->Table_Leeuwenhoek = new Table_LeeuwenhoekTable(dataSet.tables()["Table_Leeuwenhoek"]);
        this->Table_pertain = new Table_pertainTable(dataSet.tables()["Table_pertain"]);
        this->Table163 = new Table163Table(dataSet.tables()["Table163"]);
        this->Table174 = new Table174Table(dataSet.tables()["Table174"]);
        this->Table188 = new Table188Table(dataSet.tables()["Table188"]);
        this->Table_globetrotter = new Table_globetrotterTable(dataSet.tables()["Table_globetrotter"]);
        this->Table173 = new Table173Table(dataSet.tables()["Table173"]);
        this->Table187 = new Table187Table(dataSet.tables()["Table187"]);
        this->Table_Aug = new Table_AugTable(dataSet.tables()["Table_Aug"]);
        this->Table181 = new Table181Table(dataSet.tables()["Table181"]);
        this->Table49 = new Table49Table(dataSet.tables()["Table49"]);
        this->Table_cognizances = new Table_cognizancesTable(dataSet.tables()["Table_cognizances"]);
        this->Table145 = new Table145Table(dataSet.tables()["Table145"]);
        this->Table26 = new Table26Table(dataSet.tables()["Table26"]);
        this->Table_adenoid = new Table_adenoidTable(dataSet.tables()["Table_adenoid"]);
        this->Table_visualization = new Table_visualizationTable(dataSet.tables()["Table_visualization"]);
        this->Table147 = new Table147Table(dataSet.tables()["Table147"]);
        this->Table197 = new Table197Table(dataSet.tables()["Table197"]);
        this->Table_blustering = new Table_blusteringTable(dataSet.tables()["Table_blustering"]);
        this->Table_creepy = new Table_creepyTable(dataSet.tables()["Table_creepy"]);
        this->Table_teashop = new Table_teashopTable(dataSet.tables()["Table_teashop"]);
        this->Table178 = new Table178Table(dataSet.tables()["Table178"]);
        this->Table35 = new Table35Table(dataSet.tables()["Table35"]);
        this->Table41 = new Table41Table(dataSet.tables()["Table41"]);
        this->Table8 = new Table8Table(dataSet.tables()["Table8"]);
        this->Table_annoy = new Table_annoyTable(dataSet.tables()["Table_annoy"]);
        this->Table_headdress = new Table_headdressTable(dataSet.tables()["Table_headdress"]);
        this->Table_Valina = new Table_ValinaTable(dataSet.tables()["Table_Valina"]);
        this->Table_Weinberg = new Table_WeinbergTable(dataSet.tables()["Table_Weinberg"]);
        this->Table198 = new Table198Table(dataSet.tables()["Table198"]);
        this->Table_longsighted = new Table_longsightedTable(dataSet.tables()["Table_longsighted"]);
        this->Table161 = new Table161Table(dataSet.tables()["Table161"]);
        this->Table_interval = new Table_intervalTable(dataSet.tables()["Table_interval"]);
        this->Table_recopy = new Table_recopyTable(dataSet.tables()["Table_recopy"]);
        this->Table_Riva = new Table_RivaTable(dataSet.tables()["Table_Riva"]);
        this->Table_Tamarra = new Table_TamarraTable(dataSet.tables()["Table_Tamarra"]);
        this->Table190 = new Table190Table(dataSet.tables()["Table190"]);
        this->Table156 = new Table156Table(dataSet.tables()["Table156"]);
        this->Table_throb = new Table_accidentTable(dataSet.tables()["Table_throb"]);
        this->Table_nonfreezing = new Table_alienTable(dataSet.tables()["Table_nonfreezing"]);
        this->Table_penitence = new Table_alienTable(dataSet.tables()["Table_penitence"]);
        this->Table_amain = new Table_alienTable(dataSet.tables()["Table_amain"]);
        this->Table_Little = new Table_alienTable(dataSet.tables()["Table_Little"]);
        this->Table_Bodenheim = new Table_alienTable(dataSet.tables()["Table_Bodenheim"]);
        this->Table_kipping = new Table_annoyTable(dataSet.tables()["Table_kipping"]);
        this->Table_PASCAL = new Table_apocryphalnessTable(dataSet.tables()["Table_PASCAL"]);
        this->Table_undergone = new Table_approveTable(dataSet.tables()["Table_undergone"]);
        this->Table_nonuniform = new Table_bedpanTable(dataSet.tables()["Table_nonuniform"]);
        this->Table_techs = new Table_bedpanTable(dataSet.tables()["Table_techs"]);
        this->Table_incorporate = new Table_bedpanTable(dataSet.tables()["Table_incorporate"]);
        this->Table_Charolais = new Table_bedpanTable(dataSet.tables()["Table_Charolais"]);
        this->Table_overhand = new Table_blusteringTable(dataSet.tables()["Table_overhand"]);
        this->Table_fiddle = new Table_bootprintsTable(dataSet.tables()["Table_fiddle"]);
        this->Table_overdriven = new Table_capsulizeTable(dataSet.tables()["Table_overdriven"]);
        this->Table_crosspoint = new Table_capsulizeTable(dataSet.tables()["Table_crosspoint"]);
        this->Table_parlous = new Table_CaribbeanTable(dataSet.tables()["Table_parlous"]);
        this->Table_text = new Table_CarolanTable(dataSet.tables()["Table_text"]);
        this->Table_Crimea = new Table_chromatographyTable(dataSet.tables()["Table_Crimea"]);
        this->Table_Kynthia = new Table_conicalTable(dataSet.tables()["Table_Kynthia"]);
        this->Table_noble = new Table_deathlessTable(dataSet.tables()["Table_noble"]);
        this->Table_stipend = new Table_departTable(dataSet.tables()["Table_stipend"]);
        this->Table_dandelion = new Table_drawnTable(dataSet.tables()["Table_dandelion"]);
        this->Table_segregated = new Table_duxesTable(dataSet.tables()["Table_segregated"]);
        this->Table_buy = new Table_expansionaryTable(dataSet.tables()["Table_buy"]);
        this->Table_glittering = new Table_gangliaTable(dataSet.tables()["Table_glittering"]);
        this->Table_subject = new Table_gerrymanderTable(dataSet.tables()["Table_subject"]);
        this->Table_naked = new Table_GiacintaTable(dataSet.tables()["Table_naked"]);
        this->Table_hubris = new Table_halterTable(dataSet.tables()["Table_hubris"]);
        this->Table_eradication = new Table_halterTable(dataSet.tables()["Table_eradication"]);
        this->Table_primed = new Table_halterTable(dataSet.tables()["Table_primed"]);
        this->Table_McLeod = new Table_headdressTable(dataSet.tables()["Table_McLeod"]);
        this->Table_stencil = new Table_headdressTable(dataSet.tables()["Table_stencil"]);
        this->Table_Gorgonzola = new Table_inerrantTable(dataSet.tables()["Table_Gorgonzola"]);
        this->Table_Craig = new Table_intervalTable(dataSet.tables()["Table_Craig"]);
        this->Table_duplicable = new Table_ItoTable(dataSet.tables()["Table_duplicable"]);
        this->Table_defiance = new Table_ItoTable(dataSet.tables()["Table_defiance"]);
        this->Table_casein = new Table_JerryTable(dataSet.tables()["Table_casein"]);
        this->Table_nonphysical = new Table_JoycelinTable(dataSet.tables()["Table_nonphysical"]);
        this->Table_Melisent = new Table_JuddTable(dataSet.tables()["Table_Melisent"]);
        this->Table_Margery = new Table_JuddTable(dataSet.tables()["Table_Margery"]);
        this->Table_Roana = new Table_KatherynTable(dataSet.tables()["Table_Roana"]);
        this->Table_otherness = new Table_KitTable(dataSet.tables()["Table_otherness"]);
        this->Table_photogenically = new Table_LuciusTable(dataSet.tables()["Table_photogenically"]);
        this->Table_Marjory = new Table_MaxwellTable(dataSet.tables()["Table_Marjory"]);
        this->Table_Scottish = new Table_MelittaTable(dataSet.tables()["Table_Scottish"]);
        this->Table_deluge = new Table_needlessnessTable(dataSet.tables()["Table_deluge"]);
        this->Table_Shell = new Table_needlessnessTable(dataSet.tables()["Table_Shell"]);
        this->Table_embosser = new Table_quartileTable(dataSet.tables()["Table_embosser"]);
        this->Table_Yukon = new Table_refunderTable(dataSet.tables()["Table_Yukon"]);
        this->Table_imply = new Table_refunderTable(dataSet.tables()["Table_imply"]);
        this->Table_subparagraph = new Table_refunderTable(dataSet.tables()["Table_subparagraph"]);
        this->Table_downland = new Table_replenishTable(dataSet.tables()["Table_downland"]);
        this->Table_Olympian = new Table_replenishTable(dataSet.tables()["Table_Olympian"]);
        this->Table_Excalibur = new Table_repressivenessTable(dataSet.tables()["Table_Excalibur"]);
        this->Table_Rollo = new Table_repressivenessTable(dataSet.tables()["Table_Rollo"]);
        this->Table_rotate = new Table_repressivenessTable(dataSet.tables()["Table_rotate"]);
        this->Table_delfs = new Table_RivaTable(dataSet.tables()["Table_delfs"]);
        this->Table_communistic = new Table_sherbetTable(dataSet.tables()["Table_communistic"]);
        this->Table_Angelo = new Table_stockpileTable(dataSet.tables()["Table_Angelo"]);
        this->Table_tonearm = new Table_sunTable(dataSet.tables()["Table_tonearm"]);
        this->Table_pupillage = new Table_teashopTable(dataSet.tables()["Table_pupillage"]);
        this->Table_mistrust = new Table_wristTable(dataSet.tables()["Table_mistrust"]);
        this->Table_bedding = new Table1Table(dataSet.tables()["Table_bedding"]);
        this->Table_scented = new Table1Table(dataSet.tables()["Table_scented"]);
        this->Table_taxi = new Table10Table(dataSet.tables()["Table_taxi"]);
        this->Table_consular = new Table10Table(dataSet.tables()["Table_consular"]);
        this->Table_foreknown = new Table10Table(dataSet.tables()["Table_foreknown"]);
        this->Table_tremor = new Table10Table(dataSet.tables()["Table_tremor"]);
        this->Table_satisfiability = new Table10Table(dataSet.tables()["Table_satisfiability"]);
        this->Table_Stoppard = new Table10Table(dataSet.tables()["Table_Stoppard"]);
        this->Table_triableness = new Table100Table(dataSet.tables()["Table_triableness"]);
        this->Table_submerge = new Table110Table(dataSet.tables()["Table_submerge"]);
        this->Table_Hooper = new Table118Table(dataSet.tables()["Table_Hooper"]);
        this->Table_positivity = new Table118Table(dataSet.tables()["Table_positivity"]);
        this->Table_buckler = new Table13Table(dataSet.tables()["Table_buckler"]);
        this->Table_WWI = new Table13Table(dataSet.tables()["Table_WWI"]);
        this->Table_shotgunning = new Table14Table(dataSet.tables()["Table_shotgunning"]);
        this->Table_Celestyn = new Table15Table(dataSet.tables()["Table_Celestyn"]);
        this->Table_devoutness = new Table16Table(dataSet.tables()["Table_devoutness"]);
        this->Table_multivariate = new Table16Table(dataSet.tables()["Table_multivariate"]);
        this->Table_yow = new Table16Table(dataSet.tables()["Table_yow"]);
        this->Table_bigamy = new Table165Table(dataSet.tables()["Table_bigamy"]);
        this->Table_luncheon = new Table17Table(dataSet.tables()["Table_luncheon"]);
        this->Table_Morison = new Table17Table(dataSet.tables()["Table_Morison"]);
        this->Table_peahen = new Table174Table(dataSet.tables()["Table_peahen"]);
        this->Table_Tandy = new Table175Table(dataSet.tables()["Table_Tandy"]);
        this->Table_pipework = new Table18Table(dataSet.tables()["Table_pipework"]);
        this->Table_abhorrer = new Table18Table(dataSet.tables()["Table_abhorrer"]);
        this->Table_discrepant = new Table18Table(dataSet.tables()["Table_discrepant"]);
        this->Table_Dominik = new Table180Table(dataSet.tables()["Table_Dominik"]);
        this->Table_Torr = new Table183Table(dataSet.tables()["Table_Torr"]);
        this->Table_contortionist = new Table19Table(dataSet.tables()["Table_contortionist"]);
        this->Table_featherless = new Table19Table(dataSet.tables()["Table_featherless"]);
        this->Table_midtown = new Table19Table(dataSet.tables()["Table_midtown"]);
        this->Table_centigram = new Table19Table(dataSet.tables()["Table_centigram"]);
        this->Table_stalag = new Table19Table(dataSet.tables()["Table_stalag"]);
        this->Table_Beryle = new Table196Table(dataSet.tables()["Table_Beryle"]);
        this->Table_information = new Table2Table(dataSet.tables()["Table_information"]);
        this->Table_kilobyte = new Table2Table(dataSet.tables()["Table_kilobyte"]);
        this->Table_brainless = new Table2Table(dataSet.tables()["Table_brainless"]);
        this->Table_irresponsibly = new Table2Table(dataSet.tables()["Table_irresponsibly"]);
        this->Table_imperfection = new Table2Table(dataSet.tables()["Table_imperfection"]);
        this->Table_oases = new Table202Table(dataSet.tables()["Table_oases"]);
        this->Table_frolicker = new Table21Table(dataSet.tables()["Table_frolicker"]);
        this->Table_Maimonides = new Table23Table(dataSet.tables()["Table_Maimonides"]);
        this->Table_applejack = new Table24Table(dataSet.tables()["Table_applejack"]);
        this->Table_inventory = new Table24Table(dataSet.tables()["Table_inventory"]);
        this->Table_leering = new Table24Table(dataSet.tables()["Table_leering"]);
        this->Table_prefabbing = new Table24Table(dataSet.tables()["Table_prefabbing"]);
        this->Table_freewheel = new Table25Table(dataSet.tables()["Table_freewheel"]);
        this->Table_defile = new Table26Table(dataSet.tables()["Table_defile"]);
        this->Table_wiry = new Table26Table(dataSet.tables()["Table_wiry"]);
        this->Table_Adair = new Table27Table(dataSet.tables()["Table_Adair"]);
        this->Table_Broderick = new Table28Table(dataSet.tables()["Table_Broderick"]);
        this->Table_pyroxenite = new Table28Table(dataSet.tables()["Table_pyroxenite"]);
        this->Table_cutesy = new Table31Table(dataSet.tables()["Table_cutesy"]);
        this->Table_Amish = new Table31Table(dataSet.tables()["Table_Amish"]);
        this->Table_ephemerids = new Table31Table(dataSet.tables()["Table_ephemerids"]);
        this->Table_starlit = new Table32Table(dataSet.tables()["Table_starlit"]);
        this->Table_agog = new Table33Table(dataSet.tables()["Table_agog"]);
        this->Table_dribbler = new Table34Table(dataSet.tables()["Table_dribbler"]);
        this->Table_chuckling = new Table37Table(dataSet.tables()["Table_chuckling"]);
        this->Table_sandpit = new Table38Table(dataSet.tables()["Table_sandpit"]);
        this->Table_blockbusting = new Table40Table(dataSet.tables()["Table_blockbusting"]);
        this->Table_patron = new Table42Table(dataSet.tables()["Table_patron"]);
        this->Table_assumption = new Table43Table(dataSet.tables()["Table_assumption"]);
        this->Table_interscholastic = new Table43Table(dataSet.tables()["Table_interscholastic"]);
        this->Table_thigh = new Table43Table(dataSet.tables()["Table_thigh"]);
        this->Table_industrialize = new Table45Table(dataSet.tables()["Table_industrialize"]);
        this->Table_fortissimo = new Table45Table(dataSet.tables()["Table_fortissimo"]);
        this->Table_stab = new Table48Table(dataSet.tables()["Table_stab"]);
        this->Table_workaround = new Table48Table(dataSet.tables()["Table_workaround"]);
        this->Table_tiring = new Table49Table(dataSet.tables()["Table_tiring"]);
        this->Table_conciliator = new Table49Table(dataSet.tables()["Table_conciliator"]);
        this->Table_flush = new Table51Table(dataSet.tables()["Table_flush"]);
        this->Table_wavy = new Table51Table(dataSet.tables()["Table_wavy"]);
        this->Table_spicily = new Table51Table(dataSet.tables()["Table_spicily"]);
        this->Table_Gorbachev = new Table52Table(dataSet.tables()["Table_Gorbachev"]);
        this->Table_sauropod = new Table52Table(dataSet.tables()["Table_sauropod"]);
        this->Table_automation = new Table53Table(dataSet.tables()["Table_automation"]);
        this->Table_verruca = new Table58Table(dataSet.tables()["Table_verruca"]);
        this->Table_PG = new Table58Table(dataSet.tables()["Table_PG"]);
        this->Table_clockmaker = new Table58Table(dataSet.tables()["Table_clockmaker"]);
        this->Table_Hebraism = new Table6Table(dataSet.tables()["Table_Hebraism"]);
        this->Table_evict = new Table6Table(dataSet.tables()["Table_evict"]);
        this->Table_tripod = new Table6Table(dataSet.tables()["Table_tripod"]);
        this->Table_agitated = new Table60Table(dataSet.tables()["Table_agitated"]);
        this->Table_addend = new Table62Table(dataSet.tables()["Table_addend"]);
        this->Table_robbed = new Table64Table(dataSet.tables()["Table_robbed"]);
        this->Table_lassoer = new Table66Table(dataSet.tables()["Table_lassoer"]);
        this->Table_Bayonne = new Table66Table(dataSet.tables()["Table_Bayonne"]);
        this->Table_sealskin = new Table73Table(dataSet.tables()["Table_sealskin"]);
        this->Table_Antaeus = new Table76Table(dataSet.tables()["Table_Antaeus"]);
        this->Table_corn = new Table78Table(dataSet.tables()["Table_corn"]);
        this->Table_tush = new Table79Table(dataSet.tables()["Table_tush"]);
        this->Table_Pierce = new Table8Table(dataSet.tables()["Table_Pierce"]);
        this->Table_Shanghai = new Table80Table(dataSet.tables()["Table_Shanghai"]);
        this->Table_ENE = new Table80Table(dataSet.tables()["Table_ENE"]);
        this->Table_suggest = new Table83Table(dataSet.tables()["Table_suggest"]);
        this->Table_Ethyl = new Table85Table(dataSet.tables()["Table_Ethyl"]);
        this->Table_Biron = new Table88Table(dataSet.tables()["Table_Biron"]);
        this->Table_consist = new Table88Table(dataSet.tables()["Table_consist"]);
        this->Table_Hus = new Table9Table(dataSet.tables()["Table_Hus"]);
        this->Table_smashup = new Table9Table(dataSet.tables()["Table_smashup"]);
        this->Table_denouncement = new Table90Table(dataSet.tables()["Table_denouncement"]);
        this->Table_Amity = new Table96Table(dataSet.tables()["Table_Amity"]);
        this->Table_oolitic = new Table97Table(dataSet.tables()["Table_oolitic"]);
        this->Table_Bertrand = new Table97Table(dataSet.tables()["Table_Bertrand"]);
        this->Table_Dalmatia = new Table99Table(dataSet.tables()["Table_Dalmatia"]);
    }
    
    CremaDataSet::CremaDataSet(const std::string& filename)
        : CremaDataSet(CremaReader::CremaReader::ReadFromFile(filename))
    {
    }
    
    CremaDataSet::~CremaDataSet()
    {
        delete this->Table_inerrant;
        delete this->Table204;
        delete this->Table_Carol;
        delete this->Table141;
        delete this->Table40;
        delete this->Table57;
        delete this->Table206;
        delete this->Table_conical;
        delete this->Table22;
        delete this->Table_hydrosphere;
        delete this->Table46;
        delete this->Table_agapae;
        delete this->Table10;
        delete this->Table_Carolan;
        delete this->Table_repressiveness;
        delete this->Table183;
        delete this->Table94;
        delete this->Table82;
        delete this->Table87;
        delete this->Table_implacableness;
        delete this->Table_fleeing;
        delete this->Table50;
        delete this->Table53;
        delete this->Table89;
        delete this->Table109;
        delete this->Table180;
        delete this->Table20;
        delete this->Table21;
        delete this->Table43;
        delete this->Table51;
        delete this->Table108;
        delete this->Table76;
        delete this->Table_duxes;
        delete this->Table196;
        delete this->Table150;
        delete this->Table_replenish;
        delete this->Table_bedpan;
        delete this->Table_wrist;
        delete this->Table33;
        delete this->Table_drawn;
        delete this->Table_Hallinan;
        delete this->Table175;
        delete this->Table85;
        delete this->Table151;
        delete this->Table_alien;
        delete this->Table_crystallizes;
        delete this->Table112;
        delete this->Table116;
        delete this->Table199;
        delete this->Table9;
        delete this->Table_piquantness;
        delete this->Table111;
        delete this->Table28;
        delete this->Table_survey;
        delete this->Table128;
        delete this->Table31;
        delete this->Table34;
        delete this->Table_stockpile;
        delete this->Table44;
        delete this->Table67;
        delete this->Table_accident;
        delete this->Table_symbiotic;
        delete this->Table_Thornburg;
        delete this->Table_vixenish;
        delete this->Table142;
        delete this->Table96;
        delete this->Table98;
        delete this->Table_refunder;
        delete this->Table_wolves;
        delete this->Table45;
        delete this->Table83;
        delete this->Table_sherbet;
        delete this->Table_freighter;
        delete this->Table_Hanoverian;
        delete this->Table_houri;
        delete this->Table191;
        delete this->Table_lunchtime;
        delete this->Table165;
        delete this->Table_dreamless;
        delete this->Table81;
        delete this->Table_chicane;
        delete this->Table90;
        delete this->Table203;
        delete this->Table_attachment;
        delete this->Table195;
        delete this->Table_Pamirs;
        delete this->Table_kinder;
        delete this->Table_deathless;
        delete this->Table117;
        delete this->Table_selection;
        delete this->Table1;
        delete this->Table68;
        delete this->Table_emotionless;
        delete this->Table_halter;
        delete this->Table_studio;
        delete this->Table115;
        delete this->Table182;
        delete this->Table_sun;
        delete this->Table_license;
        delete this->Table129;
        delete this->Table_navigable;
        delete this->Table_anchor;
        delete this->Table_Leontine;
        delete this->Table149;
        delete this->Table17;
        delete this->Table_hoodlum;
        delete this->Table138;
        delete this->Table88;
        delete this->Table_Bonnee;
        delete this->Table_Joycelin;
        delete this->Table_reticulate;
        delete this->Table114;
        delete this->Table118;
        delete this->Table170;
        delete this->Table64;
        delete this->Table_designed;
        delete this->Table_scout;
        delete this->Table_summarizer;
        delete this->Table132;
        delete this->Table133;
        delete this->Table154;
        delete this->Table_fitted;
        delete this->Table_Tupungato;
        delete this->Table130;
        delete this->Table200;
        delete this->Table_capsulize;
        delete this->Table210;
        delete this->Table192;
        delete this->Table_adulthood;
        delete this->Table_gerrymander;
        delete this->Table193;
        delete this->Table_needlessness;
        delete this->Table24;
        delete this->Table62;
        delete this->Table_expansionary;
        delete this->Table_Giacinta;
        delete this->Table_Ianthe;
        delete this->Table_bootprints;
        delete this->Table102;
        delete this->Table164;
        delete this->Table120;
        delete this->Table158;
        delete this->Table_gynecologic;
        delete this->Table169;
        delete this->Table6;
        delete this->Table172;
        delete this->Table_computing;
        delete this->Table124;
        delete this->Table106;
        delete this->Table23;
        delete this->Table60;
        delete this->Table_Kit;
        delete this->Table189;
        delete this->Table105;
        delete this->Table15;
        delete this->Table16;
        delete this->Table32;
        delete this->Table4;
        delete this->Table48;
        delete this->Table_apocryphalness;
        delete this->Table_intactness;
        delete this->Table_Caribbean;
        delete this->Table_crupper;
        delete this->Table101;
        delete this->Table14;
        delete this->Table2;
        delete this->Table29;
        delete this->Table36;
        delete this->Table79;
        delete this->Table_Briticism;
        delete this->Table107;
        delete this->Table_Antony;
        delete this->Table140;
        delete this->Table27;
        delete this->Table_Judd;
        delete this->Table18;
        delete this->Table78;
        delete this->Table_depart;
        delete this->Table_Susanne;
        delete this->Table126;
        delete this->Table146;
        delete this->Table80;
        delete this->Table_scleroses;
        delete this->Table177;
        delete this->Table75;
        delete this->Table84;
        delete this->Table_flange;
        delete this->Table_Fonz;
        delete this->Table_Melitta;
        delete this->Table_quartile;
        delete this->Table12;
        delete this->Table202;
        delete this->Table3;
        delete this->Table63;
        delete this->Table136;
        delete this->Table66;
        delete this->Table113;
        delete this->Table52;
        delete this->Table_glide;
        delete this->Table179;
        delete this->Table_wingless;
        delete this->Table137;
        delete this->Table_Ito;
        delete this->Table_Katheryn;
        delete this->Table_Lucius;
        delete this->Table_Maxwell;
        delete this->Table_bang;
        delete this->Table_lineman;
        delete this->Table100;
        delete this->Table74;
        delete this->Table97;
        delete this->Table_archaically;
        delete this->Table_codename;
        delete this->Table_protectiveness;
        delete this->Table_dissuasive;
        delete this->Table110;
        delete this->Table_relent;
        delete this->Table_chromatography;
        delete this->Table_CPI;
        delete this->Table_metempsychoses;
        delete this->Table122;
        delete this->Table134;
        delete this->Table25;
        delete this->Table73;
        delete this->Table_oxidizes;
        delete this->Table_Kerouac;
        delete this->Table13;
        delete this->Table_ganglia;
        delete this->Table_approve;
        delete this->Table_SC;
        delete this->Table_Reade;
        delete this->Table201;
        delete this->Table_culprit;
        delete this->Table_tsunami;
        delete this->Table135;
        delete this->Table19;
        delete this->Table37;
        delete this->Table_alibi;
        delete this->Table38;
        delete this->Table58;
        delete this->Table_consanguineous;
        delete this->Table42;
        delete this->Table72;
        delete this->Table92;
        delete this->Table99;
        delete this->Table_Jerry;
        delete this->Table_Leeuwenhoek;
        delete this->Table_pertain;
        delete this->Table163;
        delete this->Table174;
        delete this->Table188;
        delete this->Table_globetrotter;
        delete this->Table173;
        delete this->Table187;
        delete this->Table_Aug;
        delete this->Table181;
        delete this->Table49;
        delete this->Table_cognizances;
        delete this->Table145;
        delete this->Table26;
        delete this->Table_adenoid;
        delete this->Table_visualization;
        delete this->Table147;
        delete this->Table197;
        delete this->Table_blustering;
        delete this->Table_creepy;
        delete this->Table_teashop;
        delete this->Table178;
        delete this->Table35;
        delete this->Table41;
        delete this->Table8;
        delete this->Table_annoy;
        delete this->Table_headdress;
        delete this->Table_Valina;
        delete this->Table_Weinberg;
        delete this->Table198;
        delete this->Table_longsighted;
        delete this->Table161;
        delete this->Table_interval;
        delete this->Table_recopy;
        delete this->Table_Riva;
        delete this->Table_Tamarra;
        delete this->Table190;
        delete this->Table156;
        delete this->Table_throb;
        delete this->Table_nonfreezing;
        delete this->Table_penitence;
        delete this->Table_amain;
        delete this->Table_Little;
        delete this->Table_Bodenheim;
        delete this->Table_kipping;
        delete this->Table_PASCAL;
        delete this->Table_undergone;
        delete this->Table_nonuniform;
        delete this->Table_techs;
        delete this->Table_incorporate;
        delete this->Table_Charolais;
        delete this->Table_overhand;
        delete this->Table_fiddle;
        delete this->Table_overdriven;
        delete this->Table_crosspoint;
        delete this->Table_parlous;
        delete this->Table_text;
        delete this->Table_Crimea;
        delete this->Table_Kynthia;
        delete this->Table_noble;
        delete this->Table_stipend;
        delete this->Table_dandelion;
        delete this->Table_segregated;
        delete this->Table_buy;
        delete this->Table_glittering;
        delete this->Table_subject;
        delete this->Table_naked;
        delete this->Table_hubris;
        delete this->Table_eradication;
        delete this->Table_primed;
        delete this->Table_McLeod;
        delete this->Table_stencil;
        delete this->Table_Gorgonzola;
        delete this->Table_Craig;
        delete this->Table_duplicable;
        delete this->Table_defiance;
        delete this->Table_casein;
        delete this->Table_nonphysical;
        delete this->Table_Melisent;
        delete this->Table_Margery;
        delete this->Table_Roana;
        delete this->Table_otherness;
        delete this->Table_photogenically;
        delete this->Table_Marjory;
        delete this->Table_Scottish;
        delete this->Table_deluge;
        delete this->Table_Shell;
        delete this->Table_embosser;
        delete this->Table_Yukon;
        delete this->Table_imply;
        delete this->Table_subparagraph;
        delete this->Table_downland;
        delete this->Table_Olympian;
        delete this->Table_Excalibur;
        delete this->Table_Rollo;
        delete this->Table_rotate;
        delete this->Table_delfs;
        delete this->Table_communistic;
        delete this->Table_Angelo;
        delete this->Table_tonearm;
        delete this->Table_pupillage;
        delete this->Table_mistrust;
        delete this->Table_bedding;
        delete this->Table_scented;
        delete this->Table_taxi;
        delete this->Table_consular;
        delete this->Table_foreknown;
        delete this->Table_tremor;
        delete this->Table_satisfiability;
        delete this->Table_Stoppard;
        delete this->Table_triableness;
        delete this->Table_submerge;
        delete this->Table_Hooper;
        delete this->Table_positivity;
        delete this->Table_buckler;
        delete this->Table_WWI;
        delete this->Table_shotgunning;
        delete this->Table_Celestyn;
        delete this->Table_devoutness;
        delete this->Table_multivariate;
        delete this->Table_yow;
        delete this->Table_bigamy;
        delete this->Table_luncheon;
        delete this->Table_Morison;
        delete this->Table_peahen;
        delete this->Table_Tandy;
        delete this->Table_pipework;
        delete this->Table_abhorrer;
        delete this->Table_discrepant;
        delete this->Table_Dominik;
        delete this->Table_Torr;
        delete this->Table_contortionist;
        delete this->Table_featherless;
        delete this->Table_midtown;
        delete this->Table_centigram;
        delete this->Table_stalag;
        delete this->Table_Beryle;
        delete this->Table_information;
        delete this->Table_kilobyte;
        delete this->Table_brainless;
        delete this->Table_irresponsibly;
        delete this->Table_imperfection;
        delete this->Table_oases;
        delete this->Table_frolicker;
        delete this->Table_Maimonides;
        delete this->Table_applejack;
        delete this->Table_inventory;
        delete this->Table_leering;
        delete this->Table_prefabbing;
        delete this->Table_freewheel;
        delete this->Table_defile;
        delete this->Table_wiry;
        delete this->Table_Adair;
        delete this->Table_Broderick;
        delete this->Table_pyroxenite;
        delete this->Table_cutesy;
        delete this->Table_Amish;
        delete this->Table_ephemerids;
        delete this->Table_starlit;
        delete this->Table_agog;
        delete this->Table_dribbler;
        delete this->Table_chuckling;
        delete this->Table_sandpit;
        delete this->Table_blockbusting;
        delete this->Table_patron;
        delete this->Table_assumption;
        delete this->Table_interscholastic;
        delete this->Table_thigh;
        delete this->Table_industrialize;
        delete this->Table_fortissimo;
        delete this->Table_stab;
        delete this->Table_workaround;
        delete this->Table_tiring;
        delete this->Table_conciliator;
        delete this->Table_flush;
        delete this->Table_wavy;
        delete this->Table_spicily;
        delete this->Table_Gorbachev;
        delete this->Table_sauropod;
        delete this->Table_automation;
        delete this->Table_verruca;
        delete this->Table_PG;
        delete this->Table_clockmaker;
        delete this->Table_Hebraism;
        delete this->Table_evict;
        delete this->Table_tripod;
        delete this->Table_agitated;
        delete this->Table_addend;
        delete this->Table_robbed;
        delete this->Table_lassoer;
        delete this->Table_Bayonne;
        delete this->Table_sealskin;
        delete this->Table_Antaeus;
        delete this->Table_corn;
        delete this->Table_tush;
        delete this->Table_Pierce;
        delete this->Table_Shanghai;
        delete this->Table_ENE;
        delete this->Table_suggest;
        delete this->Table_Ethyl;
        delete this->Table_Biron;
        delete this->Table_consist;
        delete this->Table_Hus;
        delete this->Table_smashup;
        delete this->Table_denouncement;
        delete this->Table_Amity;
        delete this->Table_oolitic;
        delete this->Table_Bertrand;
        delete this->Table_Dalmatia;
    }
    
}/*namespace cremacode*/



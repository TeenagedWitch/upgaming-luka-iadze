create table Authors (
                         ID int IDENTITY(1,1) PRIMARY KEY,
                         Name varchar(255) NOT NULL,
);

create table Books (
                       ID int IDENTITY(1,1) PRIMARY KEY,
                       Title varchar(255) NOT NULL,
                       AuthorID int foreign key references Authors(ID),
                       PublicationYear INT NOT NULL CHECK (PublicationYear <= YEAR(GETDATE()))  
);

insert into Authors (Name) values ('Robert C. Martin'), ('Jeffrey Richter');
insert into Books (Title, AuthorID, PublicationYear) values ('Clean Code', 1, 2008), ('CLR via C#', 2, 2012),
                                                            ('The Clean Coder', 1, 2011);


update Books set PublicationYear = 2013  where AuthorID = 2;

delete from Books where AuthorID = 3;

select b.Title, a.Name as AuthorName from Books b join Authors a on b.AuthorID = a.ID where PublicationYear > 2010;
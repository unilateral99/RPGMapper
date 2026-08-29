CREATE DATABASE RpgMapper
CHARSET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE RpgMapper;

CREATE TABLE sala (
codigo VARCHAR(6) PRIMARY KEY,
data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
mapa VARCHAR(300),
estado ENUM('em espera', 'em jogo') DEFAULT 'em espera',
qt_jogadores INT NOT NULL DEFAULT 0,
max_jogadores INT NOT NULL DEFAULT 6,
mapa_atual INT NOT NULL DEFAULT 0
) ENGINE = InnoDB;

CREATE TABLE jogador (
id INT AUTO_INCREMENT PRIMARY KEY,
codigo_sala VARCHAR(6),
nome VARCHAR(100),
CONSTRAINT fk_jogador_sala
	FOREIGN KEY(codigo_sala) REFERENCES sala(codigo)
    ON UPDATE CASCADE
    ON DELETE CASCADE
) ENGINE = InnoDB;

CREATE TABLE entidade (
id_entidade INT AUTO_INCREMENT PRIMARY KEY,
mapa INT DEFAULT 0,
localizacao INT,
codigo_sala VARCHAR(6),
nome VARCHAR(100),
imagem VARCHAR(300),
visibilidade BOOL,
personagem_id INT,
mapa_destino INT DEFAULT 0,
ultima_atualizacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
tipo ENUM('entidade', 'jogador', 'transicao', 'evento', 'npc') DEFAULT 'entidade',

CONSTRAINT fk_entidade_sala
	FOREIGN KEY(codigo_sala) REFERENCES sala(codigo)
    ON UPDATE CASCADE
    ON DELETE CASCADE,
CONSTRAINT fk_jogador_entidade
	FOREIGN KEY(personagem_id) REFERENCES jogador(id)
    ON DELETE SET NULL
) ENGINE = InnoDB;

CREATE TABLE mensagens (
id_mensagem INT AUTO_INCREMENT PRIMARY KEY,
texto TEXT,
codigo_sala VARCHAR(6),
id_jogador INT,
data_msg TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
CONSTRAINT fk_mensagem_sala
	FOREIGN KEY(codigo_sala) REFERENCES sala(codigo)
    ON DELETE CASCADE,
CONSTRAINT fk_mensagem_jogador
	FOREIGN KEY(id_jogador) REFERENCES jogador(id)
    ON DELETE CASCADE
) ENGINE = InnoDB;

CREATE TABLE mudancas (
id_mudanca INT AUTO_INCREMENT PRIMARY KEY,
id_entidade INT,
CONSTRAINT fk_mudanca_entidade
	FOREIGN KEY(id_entidade) REFERENCES entidade(id_entidade)
    ON DELETE CASCADE
) ENGINE = InnoDB;

DELIMITER //

CREATE PROCEDURE adicionar_entidade(
	IN p_codigo_sala VARCHAR(6),
    IN p_mapa INT,
    IN p_nome VARCHAR(100),
    IN p_imagem VARCHAR(300),
    IN p_visibilidade BOOL,
    IN p_tipo VARCHAR(20),
    IN p_localizacao INT)
    
    BEGIN
		INSERT INTO entidade(
				codigo_sala,
                mapa,
                nome,
                imagem,
                visibilidade,
                tipo,
                localizacao)
		VALUES (
				p_codigo_sala,
                p_mapa,
                p_nome,
                p_imagem,
                p_visibilidade,
                p_tipo,
                p_localizacao);
                
		SELECT LAST_INSERT_ID() AS id_entidade;
END //

CREATE PROCEDURE adicionar_mestre(IN p_codigo_sala VARCHAR(6))

BEGIN 
	INSERT INTO jogador(codigo_sala, nome) VALUES (p_codigo_sala, "Mestre");
    SELECT LAST_INSERT_ID() AS id;
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE criar_sala(
	IN p_codigo_sala VARCHAR(6),
    IN p_mapa VARCHAR(300))
    
    BEGIN
		INSERT INTO sala(
			codigo,
            mapa)
		VALUES(
			p_codigo_sala,
            p_mapa);
	END //
	
DELIMITER ;

DELIMITER //

CREATE PROCEDURE listar_jogadores(p_codigo_sala VARCHAR(6))
	BEGIN
		SELECT nome FROM jogador
        WHERE codigo_sala = p_codigo_sala
        AND nome <> "Mestre";
	END //
	
DELIMITER ;

DELIMITER //

CREATE PROCEDURE pode_entrar(
	IN p_codigo_sala VARCHAR(6),
    IN p_nome_jogador VARCHAR(100))
    
BEGIN 
    DECLARE p_max_jogadores INT;
    DECLARE p_jogadores_sala INT;
    DECLARE p_existe INT;
    
START TRANSACTION;

SELECT COUNT(*)
INTO p_existe
FROM sala
WHERE codigo = p_codigo_sala;

IF p_existe = 0 THEN ROLLBACK;
	SELECT 0 AS saida, "Sala não existe" AS mensagem;
ELSE
	SELECT max_jogadores
    INTO p_max_jogadores
    FROM sala
    WHERE codigo = p_codigo_sala;
    
    SELECT COUNT(id) INTO p_jogadores_sala FROM jogador WHERE codigo_sala = p_codigo_sala;
    
    IF p_jogadores_sala >= p_max_jogadores THEN
		ROLLBACK;
        SELECT 0 AS saida, "Sala cheia" AS mensagem;
	ELSE
		INSERT INTO jogador(nome, codigo_sala)
        VALUES (p_nome_jogador, p_codigo_sala);
        
		UPDATE sala
        SET qt_jogadores = qt_jogadores + 1
        WHERE codigo = p_codigo_sala;

		COMMIT;

		SELECT 1 AS saida, "Jogador entrou na sala" AS mensagem,
        LAST_INSERT_ID() AS id_jogador,
        (SELECT nome FROM jogador WHERE id = LAST_INSERT_ID()) AS nome;
	END IF;
END IF;

END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE listar_mensagens(
				IN p_id_ultima_msg INT,
                IN p_codigo_sala VARCHAR(6))
                
BEGIN 
	SELECT m.id_mensagem, e.nome, m.texto, m.data_msg
    FROM mensagens m
    JOIN jogador e ON m.id_jogador = e.id
    WHERE m.id_mensagem > p_id_ultima_msg
    AND p_codigo_sala = m.codigo_sala;
    
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE mover_entidade(
						IN p_id_entidade INT,
                        IN p_localizacao INT,
                        IN p_codigo_sala VARCHAR(6))
BEGIN
	DECLARE p_data_atual TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    
    UPDATE entidade SET localizacao = p_localizacao
    WHERE id_entidade = p_id_entidade;

	UPDATE entidade SET ultima_atualizacao = p_data_atual
    WHERE id_entidade = p_id_entidade;
    
    INSERT INTO mudancas(id_entidade)
    VALUES (p_id_entidade);
END //

CREATE PROCEDURE listar_novas_entidades(
					IN p_codigo_sala VARCHAR(6),
                    IN p_ultima_id INT)
BEGIN

	SELECT * FROM entidade
    WHERE id_entidade > p_ultima_id
    AND codigo_sala = p_codigo_sala;

END //

CREATE PROCEDURE listar_mudancas(
					IN p_codigo_sala VARCHAR(6),
                    IN p_ultima_atualizacao INT)
BEGIN

	SELECT m.id_mudanca, m.id_entidade, e.mapa, e.localizacao FROM mudancas m
    JOIN entidade e ON m.id_entidade = e.id_entidade
    WHERE e.codigo_sala = p_codigo_sala
    AND m.id_mudanca > p_ultima_atualizacao;

END //

CREATE PROCEDURE carregar_jogadores(
					IN p_codigo_sala VARCHAR(6),
                    IN p_nome VARCHAR(100),
                    IN p_imagem VARCHAR(300))
                    
BEGIN
	INSERT INTO entidade(codigo_sala, nome, imagem, tipo)
    VALUES (p_codigo_sala, p_nome, p_imagem, 'jogador');
    
    SELECT last_insert_id() AS id;
END //

DELIMITER ;

CALL criar_sala(8322, "mapa.json");
CALL criar_sala(1, "mapa.json");

CALL adicionar_entidade("1", 1, "Goblin", "Goblin.png", TRUE, "entidade", 12);
SELECT * FROM sala
WHERE codigo = "12";



CALL adicionar_entidade(1, NULL, "Pablo", NULL, TRUE, "jogador", NULL);
CALL adicionar_entidade(1, NULL, "Pedro", NULL, TRUE, "jogador", NULL);
CALL adicionar_entidade(1, NULL, "Marcelo_God", NULL, TRUE, "jogador", NULL);

CALL listar_jogadores("1");

CALL pode_entrar("1", "Robdaersasdodsaoodasooo");
SELECT * FROM entidade
WHERE codigo_sala = "8322";

SELECT * FROM sala;
SELECT * FROM mensagens;
SELECT * FROM entidade;


UPDATE sala SET estado = 'em jogo' WHERE codigo = "144316";

INSERT INTO mensagens(id_jogador, texto)
VALUES (5, "Eu gosto de banana!");

SELECT * FROM entidade;

CALL listar_novas_entidades("1", 2);
CALL mover_entidade(3, 1322);
CALL listar_mudancas("1", '2026-08-18 16:52:51');

DELETE FROM entidade WHERE id_entidade IN (1,2,3,4,5);

SELECT * FROM mudancas;

CALL adicionar_entidade(1,"ad","nao","asd",1,"entidade",21);

CALL listar_mudancas("716290", 0);

SELECT id_entidade FROM entidade WHERE codigo_sala = "206066" AND personagem_id IS NULL AND tipo = 'jogador' AND localizacao IS NOT NULL;

UPDATE entidade SET localizacao = 0 WHERE id_entidade = 20;

CALL mover_entidade(20, 12, 387888);

INSERT INTO jogador(codigo_sala, nome) VALUES ("1", "Marcel");
UPDATE entidade SET personagem_id = 1 WHERE id_entidade = 2;
UPDATE entidade SET personagem_id = NULL WHERE id_entidade = 2;

SELECT id_entidade, personagem_id FROM entidade WHERE codigo_sala = "29127" AND tipo = 'jogador';

SELECT mapa_atual FROM sala WHERE codigo = "9695";

DESCRIBE sala